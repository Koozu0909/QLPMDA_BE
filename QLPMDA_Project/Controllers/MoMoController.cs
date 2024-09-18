using Microsoft.AspNetCore.Mvc;
using ProGCoder_MomoAPI.Services;
using QLPMDA_Project.Models;
using QLPMDA_Project.ViewModels;

namespace QLPMDA_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoMoController : GenericController<Products>
    {
        private IMomoService _momoService;

        public MoMoController(QLPMDAContext db, IHttpContextAccessor httpContextAccessor, IMomoService momoService) : base(db, httpContextAccessor)
        {
            _momoService = momoService;
        }
        [HttpPost]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfoModel model)
        {
            var orderId = DateTime.UtcNow.Ticks.ToString();
            var order = new Order
            {
                CustomerName = model.FullName,
                TotalAmount = model.Amount,
                IdRaw = orderId

            };
            var response = await _momoService.CreatePaymentAsync(model, orderId);
            return Ok(response.PayUrl);
        }

    }

}
