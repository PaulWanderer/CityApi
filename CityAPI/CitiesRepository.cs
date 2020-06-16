using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityAPI.Models;

namespace CityAPI
{
    public class CitiesRepository
    {
        public static CitiesRepository Current { get; } = new CitiesRepository();

        public List<CityDto> Cities { get; set; }

        public CitiesRepository()
        {
            Cities = new List<CityDto>()
            {
                new CityDto()
                {
                    Id = 1,
                    Name = "Detroit",
                    Description = "The one with the cars",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 1,
                            Name = "The Fist",
                            Description = "The statue of that guys fist"
                        },
                        new PointOfInterestDto()
                        {
                            Id = 2,
                            Name = "The GM Building",
                            Description = "The big old GM buildings"
                        }

                    }
                },
                new CityDto()
                {
                    Id = 2,
                    Name = "Chicago",
                    Description = "The one with the river",
                    PointsOfInterest = new List<PointOfInterestDto>()
                    {
                        new PointOfInterestDto()
                        {
                            Id = 3,
                            Name = "The Bean",
                            Description = "The big bean looking thing"
                        },
                        new PointOfInterestDto()
                        {
                            Id = 4,
                            Name = "The Sears Tower",
                            Description = "The gigantic skyscraper"
                        }

                    }
                }
            };
        }
    }
}
