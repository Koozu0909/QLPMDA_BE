using QLPMDA_Project.Models;

namespace QLPMDA_Project.ViewModels
{
    public class OrderInfoModel
    {
        public string FullName { get; set; }
        public string OrderId { get; set; }
        public string OrderInfo { get; set; }
        public decimal Amount { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PaymentMethod { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}
