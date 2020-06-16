using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityAPI.Entities;
using CityAPI.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CityAPI.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityContext _cityContext;

        public CityInfoRepository(CityContext cityContext)
        {
            _cityContext = cityContext;
        }

        public IEnumerable<City> GetCities()
        {
            return _cityContext.Cities.OrderBy(m => m.Name);
        }

        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return _cityContext.Cities.Include(m => m.PointsOfInterest).FirstOrDefault(m => m.Id == cityId);
            }

            return _cityContext.Cities.FirstOrDefault(m => m.Id == cityId);
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterest(int cityId)
        {
            return _cityContext.PointsOfInterest.Where(m => m.CityId == cityId);
        }

        public PointOfInterest GetPointOfInterest(int cityId, int id)
        {
            return _cityContext.PointsOfInterest.FirstOrDefault(m => m.CityId == cityId && m.Id == id);
        }
    }
}
