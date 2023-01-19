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
    public class OrdersController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;

        public OrdersController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new(_Configuration.GetConnectionString("ECommerceDBConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllOrders")]
        public IActionResult GetAllOrders()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Orders", sqlConnection);
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
        [Route("GetOrdersCount")]
        public IActionResult GetOrdersCount()
        {
            string stringQuery = "SELECT COUNT(*) FROM Customers ";

            SqlCommand sqlCommand = new(stringQuery, sqlConnection);

            sqlConnection.Open();
            int orderCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(orderCount);
        }

        [HttpGet]
        [Route("GetOrderDetail/{orderId}")]
        public IActionResult GetOrderDetailById(int orderId)
        {
            if (orderId < 1)
            {
                return BadRequest("OrderId should be greater than 0");
            }
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Orders WHERE Id = @orderId", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@orderId", orderId);

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
        [Route("GetOrderDetailByOrderDate/{orderDate}")]
        public IActionResult GetOrderDetailByOrderDate(string orderDate)
        {
            var orderDateTime = DateTime.Parse(orderDate);

            if (orderDateTime > DateTime.Now)
            {
                return BadRequest("Order Date cannot be greater than current date");
            }
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Orders WHERE OrderDate = @orderDateTime", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@orderDateTime", orderDateTime);

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
        [Route("OrderAdd")]
        public IActionResult OrderAdd([FromBody] OrderDto order)
        {
            try
            {
                if (order.CustomerId < 1)
                {
                    return BadRequest("CustomerId Should be greater than 0");
                }
                if (string.IsNullOrWhiteSpace(order.ProductName))
                {
                    return BadRequest("ProductName can not be blank");
                }
                order.ProductName = order.ProductName.Trim();
                if (order.ProductName.Length < 3 || order.ProductName.Length > 30)
                {
                    return BadRequest("ProductName should be between 3 and 30 characters.");
                }
                if (order.Amount < 50)
                {
                    return BadRequest("Invalid amount, order amount should be above 50");
                }
                var orderDateTime = DateTime.Parse(order.OrderDate);

                if (orderDateTime > DateTime.Now)
                {
                    return BadRequest("Order Date cannot be greater than current date");
                }

                if (ModelState.IsValid)

                    if (ModelState.IsValid)
                    {
                        string sqlQuery = @" INSERT INTO Orders(CustomerId, OrderDate, Amount, ProductName)
                                             VALUES (@CustomerId, @OrderDate, @Amount, @ProductName)
                                             Select Scope_Identity() ";

                        SqlCommand sqlCommand = new(sqlQuery, sqlConnection);

                        sqlCommand.Parameters.AddWithValue("@CustomerId", order.CustomerId);
                        sqlCommand.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        sqlCommand.Parameters.AddWithValue("@Amount", order.Amount);
                        sqlCommand.Parameters.AddWithValue("@ProductName", order.ProductName);

                        sqlConnection.Open();
                        order.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                        sqlConnection.Close();

                        return Ok(order.Id);
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
    }
}
