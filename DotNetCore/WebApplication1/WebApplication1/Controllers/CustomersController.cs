using Microsoft.AspNetCore.Http;
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
using WebApplication1.Repositories;

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
        public IActionResult GetAllCustomers()
        {
            CustomerRepository customerRepository = new(_Configuration);
            DataTable dataTable = customerRepository.GetAllCustomers();

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
            CustomerRepository customerRepository = new(_Configuration);
            int customerCount = customerRepository.GetCustomersCount();
            return Ok(customerCount);
        }

        [HttpGet]
        [Route("GetCustomerDetailById/{CustomerId}")]
        public IActionResult GetCustomerDetailById(int customerId)
        {
            if (customerId < 1)
            {
                return BadRequest("Customer id should be greater than 0");
            }

            CustomerRepository customerRepository = new(_Configuration);
            DataTable dataTable = customerRepository.GetCustomerDetailById(customerId);

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
        [Route("GetCustomersDetailByGenderByCountry/{gender}/{country}")]
        public IActionResult GetCustomersDetailByGenderByCountry(string gender, string country)
        {
            if (gender.Length > 6)
            {
                return BadRequest("Gender should not be more than of 6 characters");
            }
            if (country.Length > 10)
            {
                return BadRequest("country should not be more than of 10 characters");
            }

            CustomerRepository customerRepository = new(_Configuration);
            DataTable dataTable = customerRepository.GetCustomersDetailByGenderByCountry(gender, country);
           
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
        [Route("CustomerRegister")]
        public IActionResult CustomerRegister([FromBody] CustomerDto customer)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(customer.FullName))
                {
                    return BadRequest("FullName can not be blank");
                }

                customer.FullName = customer.FullName.Trim();
                customer.Gender = customer.Gender.Trim();
                customer.Country = customer.Country.Trim();

                if (customer.FullName.Length < 3 || customer.FullName.Length > 30)
                {
                    return BadRequest("FullName should be between 3 and 30 characters.");
                }
                if (customer.Age <= 18)
                {
                    return BadRequest("Invalid age, customer age should be above 18");
                }
                if (string.IsNullOrWhiteSpace(customer.Country))
                {
                    return BadRequest("Country can not be blank");
                }
                if (string.IsNullOrWhiteSpace(customer.Gender))
                {
                    return BadRequest("Gender can not be blank");
                }

                if (ModelState.IsValid)
                {
                   CustomerRepository customerRepository = new(_Configuration);
                   int id = customerRepository.Add(customer);
                   return Ok(id);
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
            if (customerId < 1)
            {
                return BadRequest("Customer id should be greater than 0");
            } 

            CustomerRepository customerRepository= new(_Configuration);
            string customerFullName = customerRepository.GetCustomerFullNameById(customerId);
            return Ok(customerFullName);
        }
    }
}

