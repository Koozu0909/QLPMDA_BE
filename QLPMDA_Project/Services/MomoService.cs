using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using QLPMDA_Project.ViewModels;
using QLPMDA_Project.ViewModels.Momo;
using RestSharp;
using System.Security.Cryptography;
using System.Text;


namespace ProGCoder_MomoAPI.Services;

public class MomoService : IMomoService
{
    private readonly IOptions<MomoOptionModel> _options;

    public MomoService(IOptions<MomoOptionModel> options)
    {
        _options = options;
    }

    public async Task<MomoCreatePaymentResponseModel> CreatePaymentAsync(OrderInfoModel model, string orderString)
    {
        model.OrderId = orderString;

        model.OrderInfo = "Khách hàng: " + model.FullName + ". Nội dung: " + model.OrderInfo;

        var rawData = "accessKey=" + _options.Value.AccessKey +
        "&amount=" + model.Amount +
        "&extraData=" +
        "&ipnUrl=" + _options.Value.IpnURL +
        "&orderId=" + model.OrderId +
        "&orderInfo=" + model.OrderInfo +
        "&partnerCode=" + _options.Value.PartnerCode +
        "&redirectUrl=" + _options.Value.ReturnUrl +
        "&requestId=" + model.OrderId +
        "&requestType=" + _options.Value.RequestType;

        var signature = HmacSHA256(rawData, _options.Value.SecretKey);

        var client = new RestClient(_options.Value.MomoApiUrl);
        var request = new RestRequest() { Method = Method.Post };
        request.AddHeader("Content-Type", "application/json; charset=UTF-8");

        var requestData = new
        {
            partnerCode = _options.Value.PartnerCode,
            accessKey = _options.Value.AccessKey,
            requestId = model.OrderId,
            amount = model.Amount.ToString(),
            orderId = model.OrderId,
            orderInfo = model.OrderInfo,
            redirectUrl = _options.Value.ReturnUrl,
            extraData = "",
            ipnUrl = _options.Value.IpnURL,
            requestType = _options.Value.RequestType,
            signature = signature
        };

        request.AddParameter("application/json", JsonConvert.SerializeObject(requestData), ParameterType.RequestBody);

        var response = await client.ExecuteAsync(request);

        if (response.Content == null)
        {
            throw new Exception("Response content is null");
        }

        // Deserialize the response into the expected model
        return JsonConvert.DeserializeObject<MomoCreatePaymentResponseModel>(response.Content);
    }


    public MomoExecuteResponseModel PaymentExecuteAsync(IQueryCollection collection)
    {
        var amount = collection.First(s => s.Key == "amount").Value;
        var orderInfo = collection.First(s => s.Key == "orderInfo").Value;
        var orderId = collection.First(s => s.Key == "orderId").Value;
        return new MomoExecuteResponseModel()
        {
            Amount = amount,
            OrderId = orderId,
            OrderInfo = orderInfo
        };
    }

    public static String HmacSHA256(string inputData, string key)
    {
        byte[] keyByte = Encoding.UTF8.GetBytes(key);
        byte[] messageBytes = Encoding.UTF8.GetBytes(inputData);
        using (var hmacsha256 = new HMACSHA256(keyByte))
        {
            byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
            string hex = BitConverter.ToString(hashmessage);
            hex = hex.Replace("-", "").ToLower();
            return hex;
        }
    }
}