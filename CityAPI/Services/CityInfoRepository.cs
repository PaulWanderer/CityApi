﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityAPI.Entities;
using CityAPI.Contexts;
using Microsoft.AspNetCore.Mvc;
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
            return _cityContext.Cities.OrderBy(m => m.Id);
        }

        public City GetCity(int cityId, bool includePointsOfInterest = false)
        {
            if (includePointsOfInterest)
            {
                return _cityContext.Cities.Include(m => m.PointsOfInterest).FirstOrDefault(m => m.Id == cityId);
            }

            return _cityContext.Cities.FirstOrDefault(m => m.Id == cityId);
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterest(int cityId)
        {
            return _cityContext.PointsOfInterest.Where(i => i.CityId == cityId);
        }

        public PointOfInterest GetPointOfInterest(int cityId, int id)
        {
            return _cityContext.PointsOfInterest.FirstOrDefault(m => m.CityId == cityId && m.Id == id);

        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _cityContext.PointsOfInterest.Remove(pointOfInterest);
        }

        public bool Save()
        {
            return _cityContext.SaveChanges() >= 0;
        }

        public bool CityExists(int cityId)
        {
            return _cityContext.Cities.Any(i => i.Id == cityId);
        }
    }
}
