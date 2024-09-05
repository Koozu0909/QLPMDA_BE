using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using QLPMDA_Project.Models;
using TMS.API.Services;

namespace QLPMDA_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : GenericController<Users>
    {
        protected readonly UserService _userSvc;

        public UserController(QLPMDAContext db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
            _userSvc = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<UserService>();
        }
        [HttpGet("Admins")]
        [Authorize(Roles = "Admin")]
        public IActionResult AdminsEndPoint()
        {
            var currentUser = _userSvc.GetCurrentUser();
            return Ok($" Hi {currentUser?.UserName}, u are an {currentUser?.Role}");
        }

        [HttpGet("User")]
        [Authorize(Roles = "Admin,User")]
        public IActionResult UserEndPoint()
        {
            var currentUser = _userSvc.GetCurrentUser();
            return Ok($" Hi {currentUser?.UserName}, u are an {currentUser?.Role}");
        }

        [HttpGet("GetMe")]
        [Authorize]
        public IActionResult GetMe()
        {
            var currentUser = _userSvc.GetCurrentUser();
            return Ok(currentUser);
        }

        [HttpGet("Public")]
        public IActionResult Public()
        {
            return Ok("Hi");
        }


    }

}
