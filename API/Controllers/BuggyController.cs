using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    public class BuggyController(DataContext context) : BaseApiController
    {
        [Authorize]
        [HttpGet("auth")]
        public ActionResult<String> GetAuth()
        {
            return "secret text";
        }

        [HttpGet("not-found")]
        public ActionResult<User> GetNotFound()
        {
            var result = context.Users.Find(-1);
            if (result == null) return NotFound();
            return result;
        }

        [HttpGet("server-error")]
        public ActionResult<User> GetServerError()
        {
            var result = context.Users.Find(-1) ?? throw new Exception("A bad thing has happened");

            return result;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("This was a bad request");
        }


    }
}
