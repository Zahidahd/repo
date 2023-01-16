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

    public class StudentsController : ControllerBase
    {
        public readonly IConfiguration _Configuration;
        SqlConnection sqlConnection;

        public StudentsController(IConfiguration configuration)
        {
            _Configuration = configuration;
            sqlConnection = new(_Configuration.GetConnectionString("PracticeDBConnection").ToString());
        }

        [HttpGet]
        [Route("GetAllStudents")]
        public IActionResult GetAllStudents()
        {
            SqlDataAdapter sqlDataAdapter = new(@"SELECT Students.Name, Students.EnrollmentId, 
                                                Students.RollNumber, Colleges.CollegeName, 
                                                Courses.CourceName 
                                                FROM Students 
                                                INNER JOIN Courses ON Students.CourseId = Courses.Id INNER JOIN
                                                Colleges ON Students.CollegeId = Colleges.Id", sqlConnection);
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
        [Route("GetStudentDetailByEnrollmentId/{enrollmentId}")]
        public IActionResult GetStudentDetailByEnrollmentId(int enrollmentId)
        {
            if (enrollmentId < 1)
            {
                return BadRequest("EnrollmentId cannot be less than 1");
            }
            SqlDataAdapter sqlDataAdapter = new(@"SELECT Students.Name, Students.EnrollmentId, 
                                                Students.RollNumber, Colleges.CollegeName, 
                                                Courses.CourceName 
                                                FROM Students 
                                                INNER JOIN Courses ON Students.CourseId = Courses.Id INNER JOIN
                                                Colleges ON Students.CollegeId = Colleges.Id
                                                WHERE EnrollmentId = @enrollmentId", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@enrollmentId", enrollmentId);

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
        [Route("GetStudentCollegeNameByEnrollmentId/{enrollmentId}")]
        public IActionResult GetStudentCollegeNameByEnrollmentId(int enrollmentId)
        {
            if (enrollmentId < 1)
            {
                return BadRequest("EnrollmentId cannot be less than 1");
            }
            string sqlQuery = @"SELECT Colleges.CollegeName 
                                   FROM Students
                                   INNER JOIN Colleges ON Students.CollegeId = Colleges.Id	
                                   WHERE EnrollmentId = @enrollmentId";

            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@enrollmentId", enrollmentId);

            sqlConnection.Open();
            string collegeName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(collegeName);
        }

        [HttpGet]
        [Route("GetStudentCourseNameByEnrollmentId/{enrollmentId}")]
        public IActionResult GetStudentCourseNameByEnrollmentId(int enrollmentId)
        {
            if (enrollmentId < 1)
            {
                return BadRequest("EnrollmentId cannot be less than 1");
            }
            string sqlQuery = @"SELECT Courses.CourceName 
                                   FROM Students
                                   INNER JOIN Courses ON Students.CourseId = Courses.Id	
                                   WHERE EnrollmentId = @enrollmentId";

            var sqlCommand = new SqlCommand(sqlQuery, sqlConnection);
            sqlCommand.Parameters.AddWithValue("@enrollmentId", enrollmentId);

            sqlConnection.Open();
            string courseName = Convert.ToString(sqlCommand.ExecuteScalar());
            sqlConnection.Close();

            return Ok(courseName);
        }

        [HttpGet]
        [Route("GetStudentsNameListByCourseId/{courseId}")]
        public IActionResult GetStudentsNameListByCourseId(int courseId)
        {
            if (courseId < 1)
            {
                return BadRequest("CourseId should be greater than 0");
            }
            SqlDataAdapter sqlDataAdapter = new(@"SELECT Students.Name FROM Students
                                                  INNER JOIN Courses ON Students.CourseId = Courses.Id
                                                  WHERE CourseId = @courseId ", sqlConnection);

            sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@courseId", courseId);

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
    }
}

