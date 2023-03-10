using Microsoft.Data.SqlClient;
using System.Data;
using WebApplication1.DTO.InputDTO;
using WebApplication1.Enums;

namespace WebApplication1.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly string _connectionString;

        public ProductRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<ProductDto> GetAllProductsAsList()
        {
            List<ProductDto> products = new();

            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Products", sqlConnection);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ProductDto productDto = new();
                    productDto.Id = (int)dataTable.Rows[i]["Id"];
                    productDto.ProductName = (string)dataTable.Rows[i]["ProductName"];
                    productDto.BrandName = (string)dataTable.Rows[i]["BrandName"];
                    productDto.ProductColor = (ProductColors)dataTable.Rows[i]["ProductColor"];
                    productDto.Price = (int)dataTable.Rows[i]["Price"];

                    products.Add(productDto);
                }
                return products;
            }
        }

        public int GetProductsCount()
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = "SELECT COUNT(*) FROM Products ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlConnection.Open();
                int productCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return productCount;
            }
        }

        public ProductDto GetProductDetailById(int productId)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Products WHERE Id = @productId", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@productId", productId);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                if (dataTable.Rows.Count > 0)
                {
                    ProductDto productDto = new();
                    productDto.Id = (int)dataTable.Rows[0]["Id"];
                    productDto.ProductName = (string)dataTable.Rows[0]["ProductName"];
                    productDto.BrandName = (string)dataTable.Rows[0]["BrandName"];
                    productDto.ProductColor = (ProductColors)dataTable.Rows[0]["ProductColor"];
                    productDto.Price = (int)dataTable.Rows[0]["Price"];

                    return productDto;
                }
                else
                    return null;
            }
        }

        public List<ProductDto> GetProductsByBrandNameByPriceUpto(string brandName, int priceUpto)
        {
            List<ProductDto> products = new();

            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new(@"SELECT * FROM Products WHERE BrandName = @brandName AND
                                                    Price <= @priceUpto", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@brandName", brandName);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@priceUpto", priceUpto);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ProductDto productDto = new();
                    productDto.Id = (int)dataTable.Rows[i]["Id"];
                    productDto.ProductName = (string)dataTable.Rows[i]["ProductName"];
                    productDto.BrandName = (string)dataTable.Rows[i]["BrandName"];
                    productDto.ProductColor = (ProductColors)dataTable.Rows[i]["ProductColor"];
                    productDto.Price = (int)dataTable.Rows[i]["Price"];

                    products.Add(productDto);
                }   
                return products;
            }
        }

        public List<ProductDto> GetProductsByPriceRange(int minimumPrice, int maximumPrice)
        {
            List<ProductDto> products = new();

            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new(@" SELECT * FROM Products 
                                                    WHERE Price BETWEEN @minimumPrice AND @maximumPrice
                                                    ORDER BY Price", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@minimumPrice", minimumPrice);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@maximumPrice", maximumPrice);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ProductDto productDto = new();
                    productDto.Id = (int)dataTable.Rows[i]["Id"];
                    productDto.ProductName = (string)dataTable.Rows[i]["ProductName"];
                    productDto.BrandName = (string)dataTable.Rows[i]["BrandName"];
                    productDto.ProductColor = (ProductColors)dataTable.Rows[i]["ProductColor"];
                    productDto.Price = (int)dataTable.Rows[i]["Price"];

                    products.Add(productDto);
                }
                return products;
            }
        }

        public List<ProductDto> GetDeliverableProductsByPincode(int pincode)
        {
            List<ProductDto> products = new();

            using (SqlConnection sqlConnection = new(_connectionString))
            {
                SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Products WHERE Pincode = @pincode", sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@pincode", pincode);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ProductDto productDto = new();
                    productDto.Id = (int)dataTable.Rows[i]["Id"];
                    productDto.ProductName = (string)dataTable.Rows[i]["ProductName"];
                    productDto.BrandName = (string)dataTable.Rows[i]["BrandName"];
                    productDto.ProductColor = (ProductColors)dataTable.Rows[i]["ProductColor"];
                    productDto.Price = (int)dataTable.Rows[i]["Price"];
                    productDto.PinCode = (int)dataTable.Rows[i]["PinCode"];

                    products.Add(productDto);
                }
                return products;

            }
        }

        public List<ProductDto> GetProductsDetailByBrandNameByProductName(string brandName, string? productName)
        {
            List<ProductDto> products = new();

            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = "SELECT * FROM Products WHERE BrandName = @brandName ";

                if (!string.IsNullOrWhiteSpace(productName))
                    sqlQuery += "AND ProductName = @productName ";

                SqlDataAdapter sqlDataAdapter = new(sqlQuery, sqlConnection);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@brandName", brandName);
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@productName", productName);
                DataTable dataTable = new();
                sqlDataAdapter.Fill(dataTable);

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    ProductDto productDto = new();
                    productDto.Id = (int)dataTable.Rows[i]["Id"];
                    productDto.ProductName = (string)dataTable.Rows[i]["ProductName"];
                    productDto.BrandName = (string)dataTable.Rows[i]["BrandName"];
                    productDto.ProductColor = (ProductColors)dataTable.Rows[i]["ProductColor"];
                    productDto.Price = (int)dataTable.Rows[i]["Price"];

                    products.Add(productDto);
                }
                return products;
            }
        }

        public string GetProductNameById(int productId)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = "SELECT ProductName FROM Products WHERE Id = @productId";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@productId", productId);
                sqlConnection.Open();
                string productName = Convert.ToString(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return productName;
            }
        }

        public int Add(ProductDto product)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = @"INSERT INTO Products(ProductName, BrandName, ProductColor, Price, LaunchDate)
                        VALUES (@ProductName, @BrandName, @ProductColor, @Price, @LaunchDate)
                        Select Scope_Identity() ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@ProductName", product.ProductName);
                sqlCommand.Parameters.AddWithValue("@BrandName", product.BrandName);
                sqlCommand.Parameters.AddWithValue("@ProductColour", product.ProductColor);
                sqlCommand.Parameters.AddWithValue("@Price", product.Price);
                sqlCommand.Parameters.AddWithValue("@LaunchDate", product.LaunchDate);
                sqlConnection.Open();
                product.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                sqlConnection.Close();
                return product.Id;
            }
        }

        public void Update(ProductDto product)
        {
            using (SqlConnection sqlConnection = new(_connectionString))
            {
                string sqlQuery = @"UPDATE  Products SET ProductName = @ProductName, BrandName = @BrandName,
                        ProductColor = @ProductColor, Price = @Price, LaunchDate = @LaunchDate
                        WHERE Id = @Id ";
                SqlCommand sqlCommand = new(sqlQuery, sqlConnection);
                sqlCommand.Parameters.AddWithValue("@ProductName", product.ProductName);
                sqlCommand.Parameters.AddWithValue("@BrandName", product.BrandName);
                sqlCommand.Parameters.AddWithValue("@ProductColour", product.ProductColor);
                sqlCommand.Parameters.AddWithValue("@Price", product.Price);
                sqlCommand.Parameters.AddWithValue("@LaunchDate", product.LaunchDate);
                sqlCommand.Parameters.AddWithValue("@Id", product.Id);
                sqlConnection.Open();
                sqlCommand.ExecuteNonQuery();
                sqlConnection.Close();
            }
        }
    }
}
