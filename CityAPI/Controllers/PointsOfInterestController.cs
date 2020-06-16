using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
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

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ?? throw new ArgumentNullException(nameof(mailService));
        }

        [HttpGet]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            try
            {
                // find city
                var city = CitiesRepository.Current.Cities.FirstOrDefault(m => m.Id == cityId);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {0} was not found", cityId);
                    return NotFound();
                }

                //fidn POI
                var pointsOfInterest = city.PointsOfInterest;

                if (pointsOfInterest == null)
                {
                    return NotFound();
                }

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
            var city = CitiesRepository.Current.Cities.FirstOrDefault(m => m.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(m => m.Id == id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

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

            var city = CitiesRepository.Current.Cities.FirstOrDefault(m => m.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var finalPointOfInterestId =
                CitiesRepository.Current.Cities.SelectMany(m => m.PointsOfInterest).Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++finalPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new {cityId = cityId, id = finalPointOfInterest.Id}, pointOfInterest);
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

            var city = CitiesRepository.Current.Cities.FirstOrDefault(m => m.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestToUpdate = city.PointsOfInterest.FirstOrDefault(m => m.Id == id);

            if (pointOfInterestToUpdate == null)
            {
                return BadRequest();
            }

            pointOfInterestToUpdate.Name = pointOfInterest.Name;
            pointOfInterestToUpdate.Description = pointOfInterest.Description;

            return NoContent();
        }

        [HttpPatch("{id}")]
        public IActionResult UpdatePointOfInterestFPartial(int cityId, int id, [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> pointOfInterest)
        {
            var city = CitiesRepository.Current.Cities.FirstOrDefault(m => m.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestToUpdate = city.PointsOfInterest.FirstOrDefault(m => m.Id == id);

            if (pointOfInterestToUpdate == null)
            {
                return BadRequest();
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterestToUpdate.Name,
                Description = pointOfInterestToUpdate.Description
            };

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

            pointOfInterestToUpdate.Name = pointOfInterestToPatch.Name;
            pointOfInterestToUpdate.Description = pointOfInterestToPatch.Description;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesRepository.Current.Cities.FirstOrDefault(m => m.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterestToDelete = city.PointsOfInterest.FirstOrDefault(m => m.Id == id);

            if (pointOfInterestToDelete == null)
            {
                return BadRequest();
            }

            city.PointsOfInterest.Remove(pointOfInterestToDelete);
            string subejct = "Point Of Interest Deleted";
            string message = $"{pointOfInterestToDelete.Name} point of interest has been deleted from {city.Name}";
            _mailService.Send(subejct, message);

            return NoContent();
        }
    }
}
