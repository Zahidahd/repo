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
using WebApplication1.Enums;
using WebApplication1.Repositories;
using static WebApplication1.Enums.GenderTypes;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        public readonly IEmployeeRepository _employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        [HttpGet]
        [Route("GetAllEmployees")]
        public IActionResult GetAllEmployees()
        {
            List<EmployeeDto> employees = _employeeRepository.GetAllEmployeesAsList();

            if (employees.Count > 0)
                return Ok(employees);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetEmployeeDetailById/{id}")]
        public IActionResult GetEmployeeById(int id)
        {
            EmployeeDto employee = _employeeRepository.GetEmployeeDetailById(id);

            if (employee is not null)
                return Ok(employee);
            else
                return NotFound("No Record Found for given id");
        }

        [HttpGet]
        [Route("GetEmployeesCount")]
        public IActionResult GetEmployeesCount()
        {
            int employeeCount = _employeeRepository.GetEmployeesCount();
            return Ok(employeeCount);
        }

        [HttpGet]
        [Route("GetEmployeesDetail/{gender}/{salary}")]
        public IActionResult GetEmployeesDetailByGenderBySalary(int gender, int salary)
        {
            if (salary < 8000)
                return BadRequest("Please enter salary above 8000");

            List<EmployeeDto> employees = _employeeRepository.GetEmployeesDetailByGenderBySalary(gender, salary);

            if (employees.Count > 0)
                return Ok(employees);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetEmployeesBySalaryRange/{minimumSalary}/{maximumSalary}")]
        public IActionResult GetEmployeesBySalaryRange(int minimumSalary, int maximumSalary)
        {
            if (maximumSalary < minimumSalary)
                return BadRequest("maximum salary can not be less than minimumSalary");

            else if (minimumSalary < 8000)
                return BadRequest("Please enter minimum salary above 8000");

            else if (maximumSalary > 500000)
                return BadRequest("Please enter maximum salary less than 500000");

            List<EmployeeDto> employees = _employeeRepository.GetEmployeesBySalaryRange(minimumSalary, maximumSalary);

            if (employees.Count > 0)
                return Ok(employees);
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetEmployeeFullNameById/{EmployeeId}")]
        public IActionResult GetEmployeeFullNameById(int employeeId)
        {
            if (employeeId < 1)
                return BadRequest("Employee Id should not be less than 1");

            string employeeFullName = _employeeRepository.GetEmployeeFullNameById(employeeId);
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
                    return BadRequest(errorMessage);

                if (ModelState.IsValid)
                {
                    int id = _employeeRepository.Add(employee);
                    return Ok(id);
                }
                return BadRequest();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    if (ex.Message.Contains("UQ_Employees_MobileNumber"))
                        return BadRequest("MobileNumber already exist");

                    if (ex.Message.Contains("UQ_Employees_Email"))
                        return BadRequest("Email already exist");

                    else
                        return BadRequest("Some error at database side");
                }
                else
                    return BadRequest("Some error at database side");
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
                string errorMessage = ValidateEmployeeRegisterOrUpdate(employee, true);
                if (!string.IsNullOrEmpty(errorMessage))
                    return BadRequest(errorMessage);

                if (ModelState.IsValid)
                {
                    _employeeRepository.Update(employee);
                    return Ok("Record updated");
                }
                return BadRequest();
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627)
                {
                    if (ex.Message.Contains("UQ_Employees_MobileNumber"))
                        return BadRequest("MobileNumber already exist");

                    if (ex.Message.Contains("UQ_Employees_Email"))
                        return BadRequest("Email already exist");

                    else
                        return BadRequest("Some error at database side");
                }
                else
                    return BadRequest("Some error at database side");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", @"Unable to save changes. 
                    Try again, and if the problem persists 
                    see your system administrator.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        private string ValidateEmployeeRegisterOrUpdate(EmployeeDto employee, bool isUpdate = false)
        {
            string errorMessage = "";

            employee.FullName = employee.FullName.Trim();

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(employee.Email);

            // Approach 1
            if (isUpdate == true)
            {
                if (employee.Id < 1)
                    errorMessage = "Id can not be less than 0";
            }
            // Approach 2

            //if (isUpdate != false)
            //{
            //    if (employee.Id < 1)
            //    {
            //        errorMessage = "Id can not be less than 0";
            //    }
            //}

            // Approach 3
            //if (isUpdate == true &&  employee.Id < 1)
            //{
            //    errorMessage = "Id can not be less than 0";
            //}

            if (!match.Success)
                errorMessage = "Email is invalid";

            else if (string.IsNullOrWhiteSpace(employee.FullName))
                errorMessage = "Name can not be blank";

            else if (employee.FullName.Length < 3 || employee.FullName.Length > 30)
                errorMessage = "FullName should be between 3 and 30 characters.";

            else if (employee.Salary < 8000)
                errorMessage = "Invalid salary, employee salary should be above 8000";

            else if (!Enum.IsDefined(typeof(GenderTypes), employee.Gender))
                errorMessage = "Invalid Gender";

            return errorMessage;
        }
    }
}

