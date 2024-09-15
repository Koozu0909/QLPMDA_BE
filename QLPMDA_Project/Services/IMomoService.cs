using QLPMDA_Project.ViewModels;
using QLPMDA_Project.ViewModels.Momo;

namespace ProGCoder_MomoAPI.Services;

public interface IMomoService
{
    Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model, string orderIdString);
    MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection);
}