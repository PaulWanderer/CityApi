using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace CityAPI.Profiles
{
    public class PointOfInterestProfile : Profile

    {
        public PointOfInterestProfile()
        {
            CreateMap<Entities.PointOfInterest, Models.PointOfInterestDto>();

            CreateMap<Entities.PointOfInterest, Models.PointOfInterestForCreationDto>();

            CreateMap<Models.PointOfInterestForCreationDto, Entities.PointOfInterest>();

            CreateMap<Models.PointOfInterestForUpdateDto, Entities.PointOfInterest>()
                .ReverseMap();
        }
    }
}
