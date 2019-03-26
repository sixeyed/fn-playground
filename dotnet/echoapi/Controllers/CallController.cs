using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace echoapi.Controllers
{
    [Route("call")]
    [ApiController]
    public class CallController : ControllerBase
    {
        [HttpGet("{input}")]
        public ActionResult<string> Get(string input)
        {
            return input.ToUpper();
        }
    }
}
