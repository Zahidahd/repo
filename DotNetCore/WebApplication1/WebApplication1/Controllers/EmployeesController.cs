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

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;

        public EmployeesController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new SqlConnection(_Configuration.GetConnectionString("EmployeesDBConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllEmployees")]
        public IActionResult GetAllEmployees()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Employees", sqlConnection);
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
        [Route("GetEmployeesCount")]
        public IActionResult GetEmployeesCount()
        {
            string sqlQuery = "SELECT COUNT(*) FROM Employees ";

            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);

            sqlConnection.Open();
            int employeeCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

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
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Employees WHERE Id = @employeeId", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@employeeId", employeeId);

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
        [Route("GetEmployeeDetail/{gender}/{salary}")]
        public IActionResult GetEmployeeDetailByGenderBySalary(string gender, int salary)
        {
            if (salary < 8000)
            {
                return BadRequest("Please Enter salary above 8000");
            }
            SqlDataAdapter sqlDataAdapter;
            sqlDataAdapter = new SqlDataAdapter($@"SELECT * FROM Employees WHERE Gender = @gender 
                                                   AND Salary > @salary", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@gender", gender);
            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@salary", salary);

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
        [Route("GetEmployeeBySalaryRange/{minimumSalary}/{maximumSalary}")]
        public IActionResult GetEmployeeBySalaryRange(int minimumSalary, int maximumSalary)
        {
            if(maximumSalary < minimumSalary)
            {
                return BadRequest("maximumSalary cannot be less than minimumSalary");
            }
            if (minimumSalary < 8000)
            {
                return BadRequest("Please Enter minimumSalary salary above 8000");
            }
            if (maximumSalary > 500000)
            {
                return BadRequest("Please Enter maximumSalary salary less than 500000");
            }
            SqlDataAdapter sqlDataAdapter;
            sqlDataAdapter = new SqlDataAdapter($@" SELECT * FROM Employees 
                                                    WHERE Salary BETWEEN @minimumSalary AND @maximumSalary
                                                    ORDER BY Salary", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@minimumSalary", minimumSalary);
            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@maximumSalary", maximumSalary);

            var dataTable = new DataTable();
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
        [Route("EmployeeRegister")]
        public IActionResult EmployeeRegister([FromBody] EmployeeDto employee)
        {
            try
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(employee.Email);
                
                if (!match.Success)
                {
                    return BadRequest("Email is invalid");
                }
                if (string.IsNullOrWhiteSpace(employee.FullName))
                {
                    return BadRequest("Name can not be blank");
                }
                if (employee.FullName.Length < 3 || employee.FullName.Length > 30)
                {
                    return BadRequest("Name should be between 3 and 30 characters.");
                }
                if (employee.Salary < 8000)
                {
                    return BadRequest("Invalid salary, Employee salary should be above 8000");
                }

                if (ModelState.IsValid)

                    if (ModelState.IsValid)
                    {
                        string sqlQuery = $@"INSERT INTO Employees(FullName, Email, Gender, DateOfJoining, Salary)
                                             VALUES (@FullName, @Email, @Gender, @DateOfJoining, @Salary)
                                             Select Scope_Identity() ";

                        var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                        sqlCommand.Parameters.AddWithValue("@FullName", employee.FullName);
                        sqlCommand.Parameters.AddWithValue("@Email", employee.Email);
                        sqlCommand.Parameters.AddWithValue("@Gender", employee.Gender);
                        sqlCommand.Parameters.AddWithValue("@DateOfJoining", employee.DateOfJoining);
                        sqlCommand.Parameters.AddWithValue("@Salary", employee.Salary);

                        sqlConnection.Open();
                        employee.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                        sqlConnection.Close();

                        return Ok(employee.Id);
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
        [Route("GetEmployeeFullNameById/{EmployeeId}")]
        public IActionResult GetEmployeeFullNameById(int employeeId)
        {
            string sqlQuery = "SELECT FullName FROM Employees WHERE Id = @employeeId";

            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@employeeId", employeeId);

            sqlConnection.Open();
            string employeeFullName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(employeeFullName);
        }
    }
}

