using Microsoft.AspNetCore.Mvc;
using ProGCoder_MomoAPI.Services;
using QLPMDA_Project.Models;
using QLPMDA_Project.ViewModels;

namespace QLPMDA_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : GenericController<Order>
    {
        private IMomoService _momoService;

        public OrderController(QLPMDAContext db, IHttpContextAccessor httpContextAccessor, IMomoService momoService) : base(db, httpContextAccessor)
        {
            _momoService = momoService;
        }
        [HttpPost("payment")]
        public async Task<IActionResult> CreatePaymentUrl(OrderInfoModel model)
        {
            if (model.PaymentMethod == "momo")
            {
                var orderId = DateTime.UtcNow.Ticks.ToString();
                await SaveOrder(model, orderId);
                var response = await _momoService.CreatePaymentAsync(model, orderId);
                return Ok(response.PayUrl);
            }

            if (model.PaymentMethod == "cash")
            {
                var orderId = DateTime.UtcNow.Ticks.ToString();
                await SaveOrder(model, orderId);
                return Ok("Order success");
            }

            return BadRequest("Invalid payment method");

        }

        public async Task SaveOrder(OrderInfoModel model, string orderId)
        {
            var order = new Order
            {
                CustomerName = model.FullName,
                TotalAmount = model.Amount,
                IdRaw = orderId,
                OrderDate = DateTime.UtcNow,
                Active = true,
                InsertedDate = DateTime.UtcNow,
                InsertedBy = 1,
                PaymentMethod = model.PaymentMethod,
                IsPay = false
            };
            ctx.Order.Add(order);
            await ctx.SaveChangesAsync();

            var orderDetails = model.OrderDetails.Select(x => new OrderDetail
            {
                OrderId = order.Id,
                Price = x.Price,
                ProductName = x.ProductName,
                ProductId = x.ProductId,
                Active = true,
                Quantity = x.Quantity,
                InsertedDate = DateTime.UtcNow,
                InsertedBy = 1
            }).ToList();

            ctx.OrderDetail.AddRange(orderDetails);
            await ctx.SaveChangesAsync();
        }

        [HttpPost("payment/momo-return")]
        public IActionResult PaymentCallBack([FromQuery] string orderId)
        {
            var Order = ctx.Order.FirstOrDefault(x => x.IdRaw == orderId);
            if (Order != null)
            {
                Order.IsPay = true;
                ctx.Order.Update(Order);
                ctx.SaveChanges();
                return Ok("Order success");
            }
            return BadRequest("Order not found");
        }



    }

}
