using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityAPI.Entities;

namespace CityAPI.Services
{
    public interface ICityInfoRepository
    {
        IEnumerable<City> GetCities();

        City GetCity(int cityId, bool includePointsOfInterest);

        IEnumerable<PointOfInterest> GetPointsOfInterest(int cityId);

        PointOfInterest GetPointOfInterest(int cityId, int it);
    }
}
