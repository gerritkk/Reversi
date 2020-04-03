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
        [Route("{token}")]
        public string Get(string token)
        {
            return token;
        }

        //Api/Reversi/Beurt/<token>
        [HttpGet]
        [Route("Beurt/{token}")]
        public string GetBeurt(string token)
        {
            return token;
        }

        //Api/Reversi/Zet/
        [HttpGet]
        [Route("Zet/{token}")]
        public string GetZet(string token)
        {
            return token;
        }

        //Api/Reversi/Opgeven/
        [HttpGet]
        [Route("Opgeven/{token}")]
        public string GetOpgeven(string token)
        {
            return token;
        }
    }
}