using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ESFA.DC.FundingClaims.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class StatusController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Ok");
        }
    }
}

