using DatingApp.Data;
using DatingApp.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuggyController : BasicController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("auth")]
        [Authorize]
        public ActionResult<string> GetSecret()
        {
            return "secret";
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            return NotFound();
        }

        [HttpGet("server-error")]
        public ActionResult<string> ServerError()
        {

            var thing = _context.Users.Find(-1);
            var thingFound = thing.ToString();
            return thingFound;
          
        }

        [HttpGet("bad-request")]
        public ActionResult<string> BadRequest()
        {
            return BadRequest();
        }

    }
}
