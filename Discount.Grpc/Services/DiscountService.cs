using Discount.Grpc.Protos;
using Grpc.Core;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        public override Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = new CouponModel
            {
                Id = 1,
                ProductName = request.ProductName,
                Description = "Sample Discount",
                Amount = 50
            };

            return Task.FromResult(coupon);
        }
    }
}