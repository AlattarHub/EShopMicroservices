using Discount.Grpc.Protos;
using Grpc.Net.Client;

namespace Shopping.Aggregator.Services
{
    public class DiscountGrpcService
    {
        
        public async Task<CouponModel> GetDiscount(string productName)
        {
            var channel = GrpcChannel.ForAddress("http://discount-grpc:8080");

            var client = new DiscountProtoService.DiscountProtoServiceClient(channel);

            var request = new GetDiscountRequest
            {
                ProductName = productName
            };

            return await client.GetDiscountAsync(request);
        }
        
        
    }
}
