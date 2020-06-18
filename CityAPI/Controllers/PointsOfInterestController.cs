using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using AutoMapper;
using CityAPI.Models;
using CityAPI.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityAPI.Controllers
{
    [ApiController]
    [Route("api/cities/{cityId}/pointsofinterest")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        private const string CityNotFound = "No city was found with that ID";

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                // find city
                if (!VerifyCityExists(cityId))
                {
                    return NotFound(CityNotFound);
                }

                var pointsOfInterest = _cityInfoRepository.GetPointsOfInterest(cityId);

                return Ok(pointsOfInterest);
            }
            catch(Exception ex)
            {
                _logger.LogCritical($"An error occurred while getting points of interest for city ID of {cityId}", ex);
                return StatusCode(500, "Something went terribly wrong, batman");
            }
            
        }

        [HttpGet("{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id)
        {
            // find city
            if (!VerifyCityExists(cityId))
            {
                return NotFound(CityNotFound);
            }
            var pointOfInterest = _cityInfoRepository.GetPointOfInterest(cityId, id);

            return Ok(pointOfInterest);
        }

        [HttpPost]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "The description and name should be different");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!VerifyCityExists(cityId))
            {
                return NotFound();
            }

            var finalPointOfInterest = _mapper.Map<Entities.PointOfInterest>(pointOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            _cityInfoRepository.Save();

            var newPointOfInterest = _mapper.Map<Models.PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new {cityId = cityId, id = newPointOfInterest.Id}, newPointOfInterest);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePointOfInterestFull(int cityId, int id, [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Description", "The description and name should be different");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!VerifyCityExists(cityId))
            {
                return NotFound(CityNotFound);
            }

            var pointOfInterestToUpdate = _cityInfoRepository.GetPointOfInterest(cityId, id);

            if (pointOfInterestToUpdate == null)
            {
                return NotFound();
            }

            _mapper.Map(pointOfInterest, pointOfInterestToUpdate);

            _cityInfoRepository.Save();

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePointOfInterestFPartial(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> pointOfInterest)
        {
            if (!VerifyCityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestToUpdate = _cityInfoRepository.GetPointOfInterest(cityId, id);

            if (pointOfInterestToUpdate == null)
            {
                return BadRequest();
            }

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestForUpdateDto>(pointOfInterestToUpdate);

            pointOfInterest.ApplyTo(pointOfInterestToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (pointOfInterestToPatch.Name == pointOfInterestToPatch.Description)
            {
                ModelState.AddModelError("Description", "The name and description should be different");
            }

            if (!TryValidateModel(pointOfInterestToPatch))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(pointOfInterestToPatch, pointOfInterestToUpdate);

            _cityInfoRepository.Save();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            if (!VerifyCityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterestToDelete = _cityInfoRepository.GetPointOfInterest(cityId, id);

            if (pointOfInterestToDelete == null)
            {
                return BadRequest();
            }

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestToDelete);

            _cityInfoRepository.Save();

            string subejct = "Point Of Interest Deleted";
            string message = $"{pointOfInterestToDelete.Name} point of interest has been deleted from {pointOfInterestToDelete.CityId}";
            _mailService.Send(subejct, message);

            return NoContent();
        }

        private bool VerifyCityExists(int cityId)
        {
            return _cityInfoRepository.CityExists(cityId);
        }
    }
}
