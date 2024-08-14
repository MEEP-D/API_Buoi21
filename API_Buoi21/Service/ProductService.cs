using API_Buoi21.Model;
using Microsoft.AspNetCore.Connections;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace API_Buoi21.Service
{
    public class ProductService : IProductService
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;

        public ProductService(IDbConnectionFactory dbConnectionFactory)
        {
            _dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<Product>> GetProductsAsync()
        {
            var products = new List<Product>();

            using (var connection = _dbConnectionFactory.GetConnection())
            using (var command = new SqlCommand("SELECT * FROM Products", (SqlConnection)connection))
            {
                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        products.Add(new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            IsActive = reader.GetBoolean(4)
                        });
                    }
                }
            }

            return products;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            Product product = null;

            using (var connection = _dbConnectionFactory.GetConnection())
            using (var command = new SqlCommand("SELECT * FROM Products WHERE Id = @Id", (SqlConnection)connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        product = new Product
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            IsActive = reader.GetBoolean(4)
                        };
                    }
                }
            }

            return product;
        }

        public async Task<Product> AddProductAsync(Product product)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            using (var command = new SqlCommand(
                "INSERT INTO Products (Name, Description, Price, IsActive) " +
                "OUTPUT INSERTED.Id VALUES (@Name, @Description, @Price, @IsActive)", (SqlConnection)connection))
            {
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@IsActive", product.IsActive);

                connection.Open();
                product.Id = (int)await command.ExecuteScalarAsync();
            }

            return product;
        }

        public async Task<Product> UpdateProductAsync(int id, Product product)
        {
            using (var connection = _dbConnectionFactory.GetConnection())
            using (var command = new SqlCommand(
                "UPDATE Products SET Name = @Name, Description = @Description, Price = @Price, IsActive = @IsActive WHERE Id = @Id", (SqlConnection)connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@IsActive", product.IsActive);

                connection.Open();
                await command.ExecuteNonQueryAsync();
            }

            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            int rowsAffected;

            using (var connection = _dbConnectionFactory.GetConnection())
            using (var command = new SqlCommand("DELETE FROM Products WHERE Id = @Id", (SqlConnection)connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                connection.Open();
                rowsAffected = await command.ExecuteNonQueryAsync();
            }

            return rowsAffected > 0;
        }
    }
}
