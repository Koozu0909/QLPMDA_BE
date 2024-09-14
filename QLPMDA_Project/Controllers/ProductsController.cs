using Microsoft.AspNetCore.Mvc;

using QLPMDA_Project.Models;

namespace QLPMDA_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : GenericController<Products>
    {

        public ProductsController(QLPMDAContext db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }
        //[HttpGet("Admins")]
        //[Authorize(Roles = "Admin")]
        //public IActionResult AdminsEndPoint()
        //{
        //    var currentUser = _userSvc.GetCurrentUser();
        //    return Ok($" Hi {currentUser?.UserName}, u are an {currentUser?.Role}");
        //}

        //[HttpGet("User")]
        //[Authorize(Roles = "Admin,User")]
        //public IActionResult UserEndPoint()
        //{
        //    var currentUser = _userSvc.GetCurrentUser();
        //    return Ok($" Hi {currentUser?.UserName}, u are an {currentUser?.Role}");
        //}

        //[HttpGet("GetMe")]
        //[Authorize]
        //public IActionResult GetMe()
        //{
        //    var currentUser = _userSvc.GetCurrentUser();
        //    return Ok(currentUser);
        //}

        //[HttpGet("Public")]
        //public IActionResult Public()
        //{
        //    return Ok("Hi");
        //}

        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(ctx.Products.Where(x => x.Active == true));
        }


    }

}
