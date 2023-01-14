﻿using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;
using System.Data;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;

        public CustomersController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new(_Configuration.GetConnectionString("ECommerceDBConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllCustomers")]
        public IActionResult GetAllProducts()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Customers", sqlConnection);
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
        [Route("GetCustomersCount")]
        public IActionResult GetCustomersCount()
        {

            string sqlQuery = "SELECT COUNT(*) FROM Customers ";

            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);

            sqlConnection.Open();
            int customerCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(customerCount);
        }

        [HttpGet]
        [Route("GetCustomerDetailById/{CustomerId}")]
        public IActionResult GetCustomerDetailById(int customerId)
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Customers WHERE Id =" + customerId, sqlConnection);
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
        [Route("GetCustomerDetailByGenderBYCountry/{gender}/{country}")]
        public IActionResult GetCustomerDetailByGenderBYCountry(string gender, string country)
        {
            string sqlQuery = $"SELECT * FROM Customers WHERE Gender ='{gender}' AND Country = '{country}' ";
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
        //Changes done
        [HttpPost]
        [Route("CustomerRegister")]
        public IActionResult CustomerRegister([FromBody] CustomerDto customer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customer.FullName))
                {
                    return BadRequest("Name can not be blank");
                }
                if (customer.FullName.Length < 3 || customer.FullName.Length > 30)
                {
                    return BadRequest("Name should be between 3 and 30 characters.");
                }
                if(customer.Age <= 18)
                {
                    return BadRequest("Invalid age, customer age should be above 18");
                }

                if (ModelState.IsValid)
                {   
                    string sqlQuery = $@"
                    INSERT INTO Customers(Name, Gender, Age, Country)
                    VALUES (@FullName, @Gender, @Age, @Country)
                    Select Scope_Identity() ";

                    var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@FullName", customer.FullName);
                    sqlCommand.Parameters.AddWithValue("@Gender", customer.Gender);
                    sqlCommand.Parameters.AddWithValue("@Age", customer.Age);
                    sqlCommand.Parameters.AddWithValue("@Country", customer.Country);

                    sqlConnection.Open();
                    customer.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                    sqlConnection.Close();

                    return Ok(customer.Id);
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
        [Route("GetCustomerFullNameById/{CustomerId}")]
        public IActionResult GetCustomerFullNameById(int customerId)
        {
            string sqlQuery = @"SELECT Name FROM Customers where id = @customerId";

            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@customerId", customerId);

            sqlConnection.Open();
            string customerFullName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(customerFullName);
        }
    }
}

