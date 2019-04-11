using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace echoapi.Controllers
{    
    [ApiController]
    public class CallController : ControllerBase
    {
        [HttpPost()]
        [Route("/call")]
        public ActionResult<string> Post([FromBody]string input)
        {
            return input.ToUpper();
        }
    }
}
