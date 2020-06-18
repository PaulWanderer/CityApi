using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CityAPI.Entities;
using CityAPI.Models;
using CityAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityAPI.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CitiesController> _logger;

        private const string CityNotFound = "No city was found with that ID";

        public CitiesController(ICityInfoRepository cityRepository, IMapper mapper, ILogger<CitiesController> logger)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCities()
        {
            var cities = _cityRepository.GetCities().ToList();

            return Ok(_mapper.Map<IEnumerable<CityWithoutPoiDto>>(cities));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCity(int id, bool includePointsOfInterest = false)
        {
            // find city
            if (!VerifyCityExists(id))
            {
                return NotFound(CityNotFound);
            }

            var cityToReturn = _cityRepository.GetCity(id, includePointsOfInterest);
            
            if (includePointsOfInterest)
            {
                return Ok(_mapper.Map<CityDto>(cityToReturn));
            }

            return Ok(_mapper.Map<CityWithoutPoiDto>(cityToReturn));
        }

        private bool VerifyCityExists(int cityId)
        {
            return _cityRepository.CityExists(cityId);
        }
    }
}
