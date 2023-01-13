using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Data;

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
            sqlConnection = new SqlConnection(_Configuration.GetConnectionString("ECommerceDBConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllOrders")]
        public IActionResult GetAllOrders()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Orders", sqlConnection);
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
        [Route("GetOrdersCount")]
        public IActionResult GetOrdersCount()
        {
            string stringQuery = @"SELECT COUNT(*) FROM Customers ";

            var sqlCommand = new SqlCommand(stringQuery, sqlConnection);

            sqlConnection.Open();
            int customerCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(customerCount);
        }

        [HttpGet]
        [Route("GetOrderDetail/{orderId}")]
        public IActionResult GetOrderDetailById(int orderId)
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Orders WHERE Id =" + orderId, sqlConnection);
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
            var dateTime = DateTime.Parse(orderDate);
            string stringQuery = $" SELECT * FROM Orders WHERE OrderDate = {dateTime}";
            SqlDataAdapter sqlDataAdapter = new(stringQuery, sqlConnection); DataTable dataTable = new();
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
    }
}
