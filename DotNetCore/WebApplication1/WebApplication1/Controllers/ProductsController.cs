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
            DataTable dataTable = new DataTable();
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
                return NotFound("product Id should be greater than 0");
            }
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Products WHERE Id =" + productId, sqlConnection);
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
            string sqlQuery = $" SELECT * FROM Products WHERE BrandName = '{brandName}' AND Price <= '{priceUpto}' ";
            SqlDataAdapter sqlDataAdapter = new(sqlQuery, sqlConnection);
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

            string sqlQuery = $@" SELECT * FROM Products 
                                    WHERE Price BETWEEN {minimumPrice} AND {maximumPrice}
                                    ORDER BY Price ";
            SqlDataAdapter sqlDataAdapter = new(sqlQuery, sqlConnection);
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
            string sqlQuery = $" SELECT * FROM Products WHERE Pincode = '{pincode}'";
            SqlDataAdapter sqlDataAdapter = new(sqlQuery, sqlConnection);
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

        // This code is not running please help
        [HttpGet]
        [Route("GetProductsDetail/{brandName}/{productName}")]
        public IActionResult GetProductsDetailByProductNameBrandName(string brandName, string productName)
        {
            string sqlQuery = $"SELECT * FROM Products WHERE BrandName Like '%{brandName}%' ";

            if (productName != "")
            {
                sqlQuery = sqlQuery + "AND ProductName Like '%{productName}%' ";
            }

            SqlDataAdapter sqlDataAdapter = new(sqlQuery, sqlConnection);
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


