using Microsoft.AspNetCore.Mvc;
using QLPMDA_Project.Models;
using TMS.API.Services;

namespace QLPMDA_Project.Controllers
{
    public class GenericController<T> : ControllerBase where T : class
    {
        protected const string IdField = "Id";
        protected const string InsertedByField = "InsertedBy";
        protected QLPMDAContext ctx;
        protected IServiceProvider _serviceProvider;
        protected IConfiguration _config;
        public IWebHostEnvironment _host;
        protected readonly UserService _userSvc;
        protected readonly IHttpContextAccessor _httpContext;

        protected int UserId { get; private set; }

        public GenericController(QLPMDAContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            ctx = dbContext;
            _httpContext = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _userSvc = _httpContext.HttpContext.RequestServices.GetService(typeof(UserService)) as UserService;
            _config = _httpContext.HttpContext.RequestServices.GetService(typeof(IConfiguration)) as IConfiguration;
            CalcUserInfo();
        }
        protected void CalcUserInfo()
        {
            UserId = _userSvc.UserId;
        }
    }
}
