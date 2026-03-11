using Npgsql;

namespace Discount.API.Data
{
    public class DiscountContext
    {
        private readonly IConfiguration _configuration;

        public DiscountContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public NpgsqlConnection CreateConnection()
        {
            var connectionString = _configuration.GetConnectionString("Database");
            return new NpgsqlConnection(connectionString);
        }
    }
}
