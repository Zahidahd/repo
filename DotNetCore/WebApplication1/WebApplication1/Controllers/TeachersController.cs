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
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;

        public TeachersController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new SqlConnection(_Configuration.GetConnectionString("PracticeDBConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllTeachers")]
        public IActionResult GetTeachers()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Teachers", sqlConnection);
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
        [Route("GetTeachersCount")]
        public IActionResult GetTeachersCount()
        {
            string sqlQuery = "SELECT COUNT(*) FROM Teachers";

            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);

            sqlConnection.Open();
            int customerCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();
                
            return Ok(customerCount);
        }

        [HttpGet]   
        [Route("GetTeacherDetail/{teacherId}")]
        public IActionResult GetTeacherDetailById(int teacherId)
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Teachers WHERE Id =" + teacherId, sqlConnection);
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
        [Route("GetTeachers/{department}/{teacherName}")]
        public IActionResult GetTeacherDetail(string department, string teacherName)
        {
            string stringQuery = $"SELECT * FROM Teachers WHERE FullName ='{teacherName}' AND Department = '{department}' ";
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
        [Route("GetTeacherBySalaryRange/{minimumSalary}/{maximumSalary}")]
        public IActionResult GetTeacherBySalaryRange(int minimumSalary, int maximumSalary)
        {
            string sqlQuery = $@" SELECT * FROM Teachers 
                                    WHERE Salary BETWEEN {minimumSalary} AND {maximumSalary}
                                    ORDER BY Salary ";
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
        [Route("TeacherRegister")]
        public IActionResult TeacherRegister([FromBody] TeacherDto teacherDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(teacherDto.FullName))
                {
                    return BadRequest("Name can not be blank");
                }
                if (teacherDto.FullName.Length < 3 || teacherDto.FullName.Length > 30)
                {
                    return BadRequest("Name should be between 3 and 30 characters.");
                }
                if (teacherDto.Age <= 25)
                {
                    return BadRequest("Invalid age, Teacher age should be above 25");
                }
                if (teacherDto.Salary < 25000)
                {
                    return BadRequest("Invalid salary, Teacher salary should be above 25000");
                }

                if (ModelState.IsValid)
                {
                    string sqlQuery = $@"
                    INSERT INTO Teachers(FullName, Age, Gender, SchoolName, Department, Salary)
                    VALUES (@FullName, @Age, @Gender, @SchoolName, @Department, @Salary)
                    Select Scope_Identity() ";

                    var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
                    sqlCommand.Parameters.AddWithValue("@FullName", teacherDto.FullName);
                    sqlCommand.Parameters.AddWithValue("@Age", teacherDto.Age);
                    sqlCommand.Parameters.AddWithValue("@Gender", teacherDto.Gender);
                    sqlCommand.Parameters.AddWithValue("@SchoolName", teacherDto.SchoolName);
                    sqlCommand.Parameters.AddWithValue("@Department", teacherDto.Department);
                    sqlCommand.Parameters.AddWithValue("@Salary", teacherDto.Salary);

                    sqlConnection.Open();
                    teacherDto.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                    sqlConnection.Close();

                    return Ok(teacherDto.Id);
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
        [Route("GetTeacherFullNameById/{TeacherId}")]
        public IActionResult GetTeacherFullNameById(int teacherId)
        {
            string sqlQuery = "SELECT FullName FROM Teachers WHERE Id = @teacherId";

            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@teacherId", teacherId);

            sqlConnection.Open();
            string employeeFullName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(employeeFullName);
        }
    }
}