using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityAPI.Contexts;
using Microsoft.AspNetCore.Mvc;

namespace CityAPI.Controllers
{
    [ApiController]
    [Route("api/testdatabase")]
    public class TestController : ControllerBase
    {
        private readonly CityContext _ctx;

        public TestController(CityContext ctx)
        {
            _ctx = ctx ?? throw new ArgumentNullException(nameof(ctx));
        }

        [HttpGet]
        public IActionResult TestDatabase()
        {
            return Ok();
        }
    }
}
