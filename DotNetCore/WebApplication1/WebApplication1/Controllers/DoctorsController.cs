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

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;

        public DoctorsController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new(_Configuration.GetConnectionString("ECommerceDBConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllDoctors")]
        public IActionResult GetAllDoctors()
        {
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Doctors", sqlConnection);
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
        [Route("GetDoctorsCount")]
        public IActionResult GetDoctorsCount()
        {
            string sqlQuery = "SELECT COUNT(*) FROM Doctors";

            SqlCommand sqlCommand = new(sqlQuery, sqlConnection);

            sqlConnection.Open();
            int doctorCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(doctorCount);
        }

        [HttpGet]
        [Route("GetDoctorDetailById/{doctorId}")]
        public IActionResult GetDoctorDetailById(int doctorId)
        {
            if (doctorId < 1)
            {
                return BadRequest("Doctor Id should be greater than 0");
            }
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Doctors WHERE Id = @doctorId", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@doctorId", doctorId);

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
        [Route("GetDoctorsByDepartmentByDoctorName/{department}/{doctorName}")]
        public IActionResult GetDoctorsByDepartmentByDoctorName(string department, string doctorName)
        {
            if (string.IsNullOrWhiteSpace(doctorName))
            {
                return BadRequest("Doctor name cannot be blank");
            }
            doctorName = doctorName.Trim();
            if (doctorName.Length < 3 || doctorName.Length > 30)
            {
                return BadRequest("DoctorName should be between 3 and 30 characters.");
            }
            department = department.Trim();
            if (department.Length < 3 || department.Length > 30)
            {
                return BadRequest("Department should be between 3 and 30 characters.");
            }
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Doctors WHERE Department = @department AND Name = @doctorName ", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@department", department);
            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@doctorName", doctorName);

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
        [Route("GetDoctorsNameListByDepartment/{department}")]
        public IActionResult GetDoctorsNameListByDepartment(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
            {
                return BadRequest("Department cannot be blank");
            }
            if (department.Length < 3 || department.Length > 30)
            {
                return BadRequest("Department should be between 3 and 30 characters.");
            }
            SqlDataAdapter sqlDataAdapter = new("SELECT * FROM Doctors WHERE Department = @department", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@department", department);

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
        [Route("DoctorRegister")]
        public IActionResult DoctorRegister([FromBody] DoctorDto doctor)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(doctor.FullName))
                {
                    return BadRequest("FullName cannot be blank");
                }
                doctor.FullName = doctor.FullName.Trim();
                if (doctor.FullName.Length < 3 || doctor.FullName.Length > 30)
                {
                    return BadRequest("FullName should be between 3 and 30 characters");
                }
                if (string.IsNullOrWhiteSpace(doctor.Department))
                {
                    return BadRequest("Department cannot be blank");
                }
                doctor.Department = doctor.Department.Trim();

                if (ModelState.IsValid)
                {
                    string sqlQuery = @"INSERT INTO Doctors(Name, Department, City)
                                        VALUES(@FullName, @Department, @City)
                                        Select Scope_Identity()";

                    SqlCommand sqlCommand = new(sqlQuery, sqlConnection);

                    sqlCommand.Parameters.AddWithValue("@FullName", doctor.FullName);
                    sqlCommand.Parameters.AddWithValue("@Department", doctor.Department);
                    sqlCommand.Parameters.AddWithValue("@City", doctor.City);

                    sqlConnection.Open();
                    doctor.Id = Convert.ToInt32(sqlCommand.ExecuteScalar());
                    sqlConnection.Close();

                    return Ok(doctor.Id);
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
        [Route("GetDoctorFullNameByDoctorId/{doctorId}")]
        public IActionResult GetDoctorFullNameByDoctorId(int doctorId)
        {
            if (doctorId < 1)
            {
                return BadRequest("Doctor Id cannot be less than 1");
            }
            string sqlQuery = "SELECT Name FROM Doctors WHERE Id = @doctorId";

            SqlCommand sqlCommand = new(sqlQuery, sqlConnection);

            sqlCommand.Parameters.AddWithValue("@doctorId", doctorId);

            sqlConnection.Open();
            string doctorFullName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(doctorFullName);
        }
    }
}
