using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ReversiRestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReversiController : ControllerBase
    {
        // api/Reversi/<token> GET
        [HttpGet]
        public string Get(string token)
        {
            return "test";
        }

        //Api/Reversi/Beurt/<token>

        //Api/Reversi/Zet 

        //Api/Reversi/Opgeven
    }
}