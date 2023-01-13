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
            string stringQuery = "SELECT COUNT(*) FROM Products ";

            var sqlCommand = new SqlCommand(stringQuery, sqlConnection);
                
            sqlConnection.Open();
            int productCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(productCount);
        }

        [HttpGet]
        [Route("GetProductDetail/{productId}")]
        public IActionResult GetProductDetailById(int productId)
        {
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
            string stringQuery = $" SELECT * FROM Products WHERE BrandName = '{brandName}' AND Price <= '{priceUpto}' ";
            SqlDataAdapter sqlDataAdapter = new(stringQuery, sqlConnection);
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
            string stringQuery = $@" SELECT * FROM Products 
                                    WHERE Price BETWEEN {minimumPrice} AND {maximumPrice}
                                    ORDER BY Price ";
            SqlDataAdapter sqlDataAdapter = new(stringQuery, sqlConnection);
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
            string stringQuery = $" SELECT * FROM Products WHERE Pincode = '{pincode}'";
            SqlDataAdapter sqlDataAdapter = new(stringQuery, sqlConnection);
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
        [Route("GetProductsDetail/{brandName}/{productName}")]
        public IActionResult GetProductsDetailByProductNameBrandName(string brandName, string productName)
        {
            string stringQuery = $"SELECT * FROM Products WHERE BrandName Like '%{brandName}%' ";

            if (productName != "")
            {
                stringQuery = stringQuery + "AND ProductName Like '%{productName}%' ";
            }

            SqlDataAdapter sqlDataAdapter = new(stringQuery, sqlConnection);
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
                if (ModelState.IsValid) 
                {
                    string stringQuery = $@"
                    INSERT INTO Products(ProductName, BrandName, Price, LaunchDate)
                    VALUES (@ProductName, @BrandName, @Price, @LaunchDate)
                    Select Scope_Identity() ";

                    var sqlCommand = new SqlCommand(stringQuery, sqlConnection);
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
            string insertQuery = "SELECT ProductName FROM Products WHERE Id = @productId";

            var sqlCommand = new SqlCommand(insertQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@productId", productId);

            sqlConnection.Open();
            string productName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(productName);
        }
    }
}


