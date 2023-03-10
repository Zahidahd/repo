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
using static WebApplication1.Enums.GenderTypes;
using System.Text.RegularExpressions;
using WebApplication1.Enums;
using System.Diagnostics.Metrics;
using System.Text;
using WebApplication1.Helpers;

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
        public IActionResult GetAllCustomersAsList()
        {
            List<CustomerDto> customers = _customerRepository.GetAllCustomersAsList();

            if (customers.Count > 0)
                return Ok(customers);
            else
                return NotFound();
        }
     
        [HttpGet]
        [Route("GetCustomerDetailById/{CustomerId}")]
        public IActionResult GetCustomerDetailById(int customerId)
        {
            if (customerId < 1)
                return BadRequest("Customer id should be greater than 0");

            CustomerDto customer = _customerRepository.GetCustomerDetailById(customerId);

            if (customer is not null)
                return Ok(customer);
            else
                return NotFound("No Record Found for given id");
        }

        [HttpGet]
        [Route("GetCustomersCount")]
        public IActionResult GetCustomersCount()
        {
            int customerCount = _customerRepository.GetCustomersCount();
            return Ok(customerCount);
        }

        [HttpGet]
        [Route("GetCustomersDetailByGenderByCountry/{gender}/{country}")]
        public IActionResult GetCustomersDetailByGenderByCountry(string gender, string country)
        {
            if (gender.Length > 6)
                return BadRequest("Gender should not be more than of 6 characters");

            else if (country.Length > 10)
                return BadRequest("country should not be more than of 10 characters");

            List<CustomerDto> customers = _customerRepository.GetCustomersDetailByGenderByCountry(gender, country);

            if (customers.Count > 0)
                return Ok(customers);
            else
                return NotFound();
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

        [HttpGet]
        [Route("Login/{email}/{password}")]
        public IActionResult Login(string email, string password)
        {
            byte[] hashValuePassword = StringHelper.StringToByteArray(password);
            CustomerDto customer = _customerRepository.GetCustomerDetailsByEmailAndPassword(email, hashValuePassword);

            if (customer is null)
            {
                _customerRepository.UpdateOnLoginFailed(email)
;
                int aleadyFailedCountInDB = _customerRepository.GetLoginFailedCount(email)
;
                if (aleadyFailedCountInDB > 1)
                {
                    _customerRepository.UpdateIsLocked(email);
                }
                return NotFound("Invalid Email or Password");
            }

            if (customer.IsLocked)
                return Ok("your account has been locked, kindly contact system administrator");

            _customerRepository.UpdateOnLoginSuccessfull(email);
            return Ok("Login Successfull");
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
                    int customerEmailCount = _customerRepository.GetEmailCount(customer);
                    int customerMobileNumberCount = _customerRepository.GetMobileNumberCount(customer);
                    if (customerEmailCount > 0)
                        return Ok("Email already exist");

                    if (customerMobileNumberCount > 0)
                        return Ok("MobileNumber already exist");
                    else
                    {
                        int id = _customerRepository.Add(customer);
                        return Ok(id);
                    }
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
                    int customerEmailCount = _customerRepository.GetEmailCount(customer);
                    int customerMobileNumberCount = _customerRepository.GetMobileNumberCount(customer);
                    if (customerEmailCount > 0)
                        return Ok("Email already exist");

                    if (customerMobileNumberCount > 0)
                        return Ok("MobileNumber already exist");

                    else
                    {
                        _customerRepository.Update(customer);
                        return Ok("Record updated");
                    }
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

        private string validateCustomerRegisterOrUpdate(CustomerDto customer, bool isUpdate = false)
        {
            string errorMessage = "";

            customer.FullName = customer.FullName.Trim();
            customer.Country = customer.Country.Trim();

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(customer.Email);

            if (isUpdate == true)
            {
                if (customer.Id < 1)
                    errorMessage = "Id can not be less than 0";
            }

            if (!match.Success)
                errorMessage = "Email is invalid";

            else if (string.IsNullOrWhiteSpace(customer.FullName))
                errorMessage = "FullName can not be blank";

            else if (customer.FullName.Length < 3 || customer.FullName.Length > 30)
                errorMessage = "FullName should be between 3 and 30 characters.";

            else if (customer.Age <= 18)
                errorMessage = "Invalid age, customer age should be above 18";

            else if (string.IsNullOrWhiteSpace(customer.Country))
                errorMessage = "Country can not be blank";

            else if (!Enum.IsDefined(typeof(GenderTypes), customer.Gender))
                errorMessage = "Invalid Gender";

            return errorMessage;
        }       
    }
}
