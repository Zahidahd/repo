using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;

        public ProductsController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new SqlConnection(_Configuration.GetConnectionString("ECommerceDBConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Products", sqlConnection);
            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("GetProductsCount")]
        public IActionResult GetProductsCount()
        {
            string sqlQuery = "SELECT COUNT(*) FROM Products ";

            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);

            sqlConnection.Open();
            int productCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(productCount);
        }

        [HttpGet]
        [Route("GetProductDetail/{productId}")]
        public IActionResult GetProductDetailById(int productId)
        {
            if (productId < 1)
            {
                return NotFound("ProductId should be greater than 0");
            }
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Products WHERE Id = @productId", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@productId", productId);

            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("GetProducts/{brandName}/{priceUpto}")]
        public IActionResult GetProductsDetailByBrandNameByPriceUpto(string brandName, int priceUpto)
        {
            if (string.IsNullOrWhiteSpace(brandName))
            {
                return BadRequest("BrandName can not be blank");
            }
            if (brandName.Length < 3 || brandName.Length > 30)
            {
                return BadRequest("BrandName should be between 3 and 30 characters.");
            }
            if (priceUpto < 600)
            {
                return BadRequest("priceUpto should be greater than 600");
            }
            SqlDataAdapter sqlDataAdapter = new($@"SELECT * FROM Products WHERE BrandName = @brandName AND
                                                    Price <= @priceUpto", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@brandName", brandName);
            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@priceUpto", priceUpto);

            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("GetProductsByPriceRange/{minimumPrice}/{maximumPrice}")]
        public IActionResult GetProductsByPriceRange(int minimumPrice, int maximumPrice)
        {
            if (maximumPrice < minimumPrice)
            {
                return BadRequest("Maximum price cannot be smaller than minimum price");
            }

            SqlDataAdapter sqlDataAdapter = new($@" SELECT * FROM Products 
                                                    WHERE Price BETWEEN @minimumPrice AND @maximumPrice
                                                    ORDER BY Price", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@minimumPrice", minimumPrice);
            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@maximumPrice", maximumPrice);

            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("GetDeliverableProductsByPincode/{pincode}")]
        public IActionResult GetDeliverableProductsByPincode(int pincode)
        {
            if (pincode.ToString().Length > 6 || pincode.ToString().Length < 6)
            {
                return BadRequest("Pincode code should be of six digits only");
            }
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Products WHERE Pincode = @pincode", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@pincode", pincode);

            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet]
        [Route("GetProductsDetailByBrandNameByProductName/{brandName}/{productName?}")]
        public IActionResult GetProductsDetailByBrandNameByProductName(string brandName, string? productName)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                return BadRequest("ProductName can not be blank");
            }
            if (productName.Length < 3 || productName.Length > 30)
            {
                return BadRequest("ProductName should be between 3 and 30 characters.");
            }

            string sqlQuery = "SELECT * FROM Products WHERE BrandName = @brandName ";

            if (!string.IsNullOrWhiteSpace(productName))
            {
                sqlQuery += "AND ProductName = @productName ";
            }

            SqlDataAdapter sqlDataAdapter = new(sqlQuery, sqlConnection);
            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@brandName", brandName);
            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@productName", productName);

            DataTable dataTable = new();
            sqlDataAdapter.Fill(dataTable);

            if (dataTable.Rows.Count > 0)
            {
                return Ok(JsonConvert.SerializeObject(dataTable));
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Route("ProductAdd")]
        public IActionResult ProductAdd([FromBody] ProductDto product)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(product.ProductName))
                {
                    return BadRequest("ProductName can not be blank");
                }
                if (product.ProductName.Length < 3 || product.ProductName.Length > 30)
                {
                    return BadRequest("ProductName should be between 3 and 30 characters.");
                }
                if (product.Price < 800)
                {
                    return BadRequest("product price should be minimum 800 or  more than 800");
                }

                if (ModelState.IsValid)
                {
                    string sqlQuery = $@"
                    INSERT INTO Products(ProductName, BrandName, Price, LaunchDate)
                    VALUES (@ProductName, @BrandName, @Price, @LaunchDate)
                    Select Scope_Identity() ";

                    var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@ProductName", product.ProductName);
                    sqlCommand.Parameters.AddWithValue("@BrandName", product.BrandName);
                    sqlCommand.Parameters.AddWithValue("@Price", product.Price);
                    sqlCommand.Parameters.AddWithValue("@LaunchDate", product.LaunchDate);

                    sqlConnection.Open();
                    product.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                    sqlConnection.Close();

                    return Ok(product.Id);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", @"Unable to save changes. 
                    Try again, and if the problem persists 
                    see your system administrator.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet]
        [Route("GetProductNameById/{ProductId}")]
        public IActionResult GetProductNameById(int productId)
        {
            if (productId < 1)
            {
                return NotFound("product Id should be greater than 0");
            }
            string sqlQuery = "SELECT ProductName FROM Products WHERE Id = @productId";

            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@productId", productId);

            sqlConnection.Open();
            string productName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(productName);
        }
    }
}


