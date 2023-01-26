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
        ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        [HttpGet]
        [Route("GetAllCustomers")]
        public IActionResult GetAllCustomers()
        {
            DataTable dataTable = _customerRepository.GetAllCustomers();

            if (dataTable.Rows.Count > 0)
                return Ok(JsonConvert.SerializeObject(dataTable));
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetCustomersCount")]
        public IActionResult GetCustomersCount()
        {
            int customerCount = _customerRepository.GetCustomersCount();
            return Ok(customerCount);
        }

        [HttpGet]
        [Route("GetCustomerDetailById/{CustomerId}")]
        public IActionResult GetCustomerDetailById(int customerId)
        {
            if (customerId < 1)
                return BadRequest("Customer id should be greater than 0");

            DataTable dataTable = _customerRepository.GetCustomerDetailById(customerId);

            if (dataTable.Rows.Count > 0)
                return Ok(JsonConvert.SerializeObject(dataTable));
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetCustomersDetailByGenderByCountry/{gender}/{country}")]
        public IActionResult GetCustomersDetailByGenderByCountry(string gender, string country)
        {
            if (gender.Length > 6)
                return BadRequest("Gender should not be more than of 6 characters");

            if (country.Length > 10)
                return BadRequest("country should not be more than of 10 characters");

            DataTable dataTable = _customerRepository.GetCustomersDetailByGenderByCountry(gender, country);

            if (dataTable.Rows.Count > 0)
                return Ok(JsonConvert.SerializeObject(dataTable));
            else
                return NotFound();
        }

        [HttpPost]
        [Route("CustomerRegister")]
        public IActionResult CustomerRegister([FromBody] CustomerDto customer)
        {
            try
            {
                string errorMessage = validateCustomerRegisterOrUpdate(customer);
                if (!string.IsNullOrEmpty(errorMessage))
                    return BadRequest(errorMessage);

                if (ModelState.IsValid)
                {
                    int id = _customerRepository.Add(customer);
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

        private string validateCustomerRegisterOrUpdate(CustomerDto customer, bool isUpdate = false)
        {
            string errorMessage = "";

            customer.FullName = customer.FullName.Trim();
            customer.Gender = customer.Gender.Trim();
            customer.Country = customer.Country.Trim();

            if (isUpdate == true)
            {
                if (customer.Id < 1)
                    errorMessage = "Id can not be less than 0";
            }

            if (string.IsNullOrWhiteSpace(customer.FullName))
                errorMessage = "FullName can not be blank";

            else if (customer.FullName.Length < 3 || customer.FullName.Length > 30)
                errorMessage = "FullName should be between 3 and 30 characters.";

            else if (customer.Age <= 18)
                errorMessage = "Invalid age, customer age should be above 18";

            else if (string.IsNullOrWhiteSpace(customer.Country))
                errorMessage = "Country can not be blank";

            else if (string.IsNullOrWhiteSpace(customer.Gender))
                errorMessage = "Gender can not be blank";

            return errorMessage;
        }

        [HttpGet]
        [Route("GetCustomerFullNameById/{CustomerId}")]
        public IActionResult GetCustomerFullNameById(int customerId)
        {
            if (customerId < 1)
                return BadRequest("Customer id should be greater than 0");

            string customerFullName = _customerRepository.GetCustomerFullNameById(customerId);
            return Ok(customerFullName);
        }

        [HttpPost]
        [Route("CustomerUpdate")]
        public IActionResult CustomerUpdate([FromBody] CustomerDto customer)
        {
            try
            {
                string errorMessage = validateCustomerRegisterOrUpdate(customer, true);
                if (!string.IsNullOrEmpty(errorMessage))
                    return BadRequest(errorMessage);

                if (ModelState.IsValid)
                {
                    _customerRepository.Update(customer);
                    return Ok("Record updated");
                }
                return BadRequest("Record not updated");
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
