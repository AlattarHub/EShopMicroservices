using Dapper;
using Discount.API.Data;
using Discount.API.Entities;

namespace Discount.API.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly DiscountContext _context;

        public DiscountRepository(DiscountContext context)
        {
            _context = context;
        }

        public async Task<Coupon> GetDiscount(string productName)
        {
            using var connection = _context.CreateConnection();

            var query = "SELECT * FROM Coupon WHERE ProductName=@ProductName";

            return await connection.QueryFirstOrDefaultAsync<Coupon>(query, new { ProductName = productName });
        }

        public async Task<bool> CreateDiscount(Coupon coupon)
        {
            using var connection = _context.CreateConnection();

            var query = "INSERT INTO Coupon(ProductName, Description, Amount) VALUES(@ProductName, @Description, @Amount)";

            var affectedRows = await connection.ExecuteAsync(query, coupon);

            return affectedRows != 0;
        }

        public async Task<bool> UpdateDiscount(Coupon coupon)
        {
            using var connection = _context.CreateConnection();

            var query = "UPDATE Coupon SET Description=@Description, Amount=@Amount WHERE ProductName=@ProductName";

            var affectedRows = await connection.ExecuteAsync(query, coupon);

            return affectedRows != 0;
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            using var connection = _context.CreateConnection();

            var query = "DELETE FROM Coupon WHERE ProductName=@ProductName";

            var affectedRows = await connection.ExecuteAsync(query, new { ProductName = productName });

            return affectedRows != 0;
        }

    }
}
