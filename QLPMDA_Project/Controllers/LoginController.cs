using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QLPMDA_Project.Extensions;
using QLPMDA_Project.Models;
using QLPMDA_Project.ViewModels;
using TMS.API.Services;

namespace QLPMDA_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : GenericController<Users>
    {
        private IConfiguration _configuration;
        protected readonly UserService _userSvc;


        public LoginController(QLPMDAContext db, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(db, httpContextAccessor)
        {
            _configuration = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            _userSvc = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<UserService>();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] LoginVM user)
        {
            var salt = _userSvc.GenerateRandomToken();
            var hashedPassword = _userSvc.GetHash(UserUtils.sHA256, user.Password);
            var newUser = new Users
            {
                UserName = user.UserName,
                Password = hashedPassword,
                Salt = salt,
                Role = "User",
                Active = true,
                HasVerifiedEmail = false,
                InsertedDate = DateTime.Now,
                InsertedBy = 1
            };
            ctx.Users.Add(newUser);
            ctx.SaveChanges();
            return Ok(newUser);
        }


        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginVM userLogin)
        {
            var user = Authenticate(userLogin);

            if (user != null)
            {
                var token = _userSvc.Generate(user);
                return Ok(new { Token = token });
            }
            else
            {
                return Unauthorized();
            }
        }

        private Users? Authenticate(LoginVM userLogin)
        {
            var hashPass = _userSvc.GetHash(UserUtils.sHA256, userLogin.Password);
            var currentUser = ctx.Users.FirstOrDefault(x => x.UserName == userLogin.UserName && x.Password == hashPass);
            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }
    }

}
