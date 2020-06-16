using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityAPI.Controllers
{
    [ApiController]
    [Route("api/cities")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityRepository;

        public CitiesController(ICityInfoRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }
        

        [HttpGet]
        public IActionResult GetCities()
        {
            var cities = _cityRepository.GetCities();
            return Ok(cities);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id)
        {
            // find city
            var cityToReturn = CitiesRepository.Current.Cities.FirstOrDefault(m => m.Id == id);

            if (cityToReturn == null)
            {
                return NotFound();
            }

            return Ok(cityToReturn);
        }
    }
}
