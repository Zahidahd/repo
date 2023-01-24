using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;
using WebApplication1.DTO.InputDTO;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        public readonly IConfiguration _Configuration;

        public EmployeesController(IConfiguration configuration)
        {
            _Configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllEmployees")]
        public IActionResult GetAllEmployees()
        {
            EmployeeRepository employeeRepository = new(_Configuration);
            DataTable dataTable = employeeRepository.GetAllEmployees();

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
        [Route("GetEmployeesCount")]
        public IActionResult GetEmployeesCount()
        {
            EmployeeRepository employeeRepository = new(_Configuration);
            int employeeCount = employeeRepository.GetEmployeesCount();
            return Ok(employeeCount);
        }

        [HttpGet]
        [Route("GetEmployeeDetailById/{employeeId}")]
        public IActionResult GetEmployeeDetailById(int employeeId)
        {
            if (employeeId < 1)
            {
                return BadRequest("EmployeeId should be greater than 0");
            }

            EmployeeRepository employeeRepository = new(_Configuration);
            DataTable dataTable = employeeRepository.GetEmployeeDetailById(employeeId);

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
        [Route("GetEmployeesDetail/{gender}/{salary}")]
        public IActionResult GetEmployeesDetailByGenderBySalary(string gender, int salary)
        {
            if (salary < 8000)
            {
                return BadRequest("Please enter salary above 8000");
            }

            EmployeeRepository employeeRepository = new(_Configuration);
            DataTable dataTable = employeeRepository.GetEmployeesDetailByGenderBySalary(gender, salary);

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
        [Route("GetEmployeesBySalaryRange/{minimumSalary}/{maximumSalary}")]
        public IActionResult GetEmployeesBySalaryRange(int minimumSalary, int maximumSalary)
        {
            if (maximumSalary < minimumSalary)
            {
                return BadRequest("maximum salary can not be less than minimumSalary");
            }
            if (minimumSalary < 8000)
            {
                return BadRequest("Please enter minimum salary above 8000");
            }
            if (maximumSalary > 500000)
            {
                return BadRequest("Please enter maximum salary less than 500000");
            }

            EmployeeRepository employeeRepository = new(_Configuration);
            DataTable dataTable = employeeRepository.GetEmployeesBySalaryRange(minimumSalary, maximumSalary);

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
        [Route("GetEmployeeFullNameById/{EmployeeId}")]
        public IActionResult GetEmployeeFullNameById(int employeeId)
        {
            if (employeeId < 1)
            {
                return BadRequest("Employee Id should not be less than 1");
            }

            EmployeeRepository employeeRepository = new(_Configuration);
            string employeeFullName = employeeRepository.GetEmployeeFullNameById(employeeId);

            return Ok(employeeFullName);
        }

        [HttpPost]
        [Route("EmployeeRegister")]
        public IActionResult EmployeeRegister([FromBody] EmployeeDto employee)
        {
            try
            {
                string errorMessage = ValidateEmployeeRegisterOrUpdate(employee);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                if (ModelState.IsValid)
                {
                    EmployeeRepository employeeRepository= new(_Configuration);
                    int id = employeeRepository.Add(employee);
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

        [HttpPost]
        [Route("EmployeeUpdate")]
        public IActionResult EmployeeUpdate([FromBody] EmployeeDto employee)
        {
            try
            {
                string errorMessage = ValidateEmployeeRegisterOrUpdate(employee);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                if (ModelState.IsValid)
                {
                    EmployeeRepository employeeRepository = new(_Configuration);
                    employeeRepository.Update(employee);
                    return Ok("Record updated");
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

        private string ValidateEmployeeRegisterOrUpdate(EmployeeDto employee)
        {
            string errorMessage = "";

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(employee.Email);
            if (!match.Success)
            {
                errorMessage = "Email is invalid";
            }
            if (string.IsNullOrWhiteSpace(employee.FullName))
            {
                errorMessage = "Name can not be blank";
            }

            employee.FullName = employee.FullName.Trim();
            employee.Gender = employee.Gender.Trim();

            if (employee.FullName.Length < 3 || employee.FullName.Length > 30)
            {
                errorMessage = "FullName should be between 3 and 30 characters.";
            }
            if (employee.Salary < 8000)
            {
                errorMessage = "Invalid salary, employee salary should be above 8000";
            }

            return errorMessage;
        }
    }
}

