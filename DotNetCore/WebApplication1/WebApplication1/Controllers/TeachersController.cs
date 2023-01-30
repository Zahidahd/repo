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
using System.Diagnostics.Metrics;
using System.Reflection;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeachersController : ControllerBase
    {
        ITeacherRepository _teacherRepository;

        public TeachersController(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        [HttpGet]
        [Route("GetAllTeachers")]
        public IActionResult GetTeachers()
        {
            DataTable dataTable = _teacherRepository.GetAllTeachers();

            if (dataTable.Rows.Count > 0)
                return Ok(JsonConvert.SerializeObject(dataTable));
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetTeachersCount")]
        public IActionResult GetTeachersCount()
        {
            int customerCount = _teacherRepository.GetTeachersCount();
            return Ok(customerCount);
        }

        [HttpGet]
        [Route("GetTeacherDetail/{teacherId}")]
        public IActionResult GetTeacherDetailById(int teacherId)
        {
            if (teacherId < 1)
                return BadRequest("TeacherId should be greater than 0");

            DataTable dataTable = _teacherRepository.GetTeacherDetailById(teacherId);

            if (dataTable.Rows.Count > 0)
                return Ok(JsonConvert.SerializeObject(dataTable));
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetTeachersByDepartmentByTeacherName/{department}/{teacherName}")]
        public IActionResult GetTeachersByDepartmentByTeacherName(string department, string teacherName)
        {
            if (string.IsNullOrWhiteSpace(teacherName))
                return BadRequest("TeacherName can not be blank");

            else if (teacherName.Length < 3 || teacherName.Length > 30)
                return BadRequest("TeacherName should be between 3 and 30 characters.");

            else if (string.IsNullOrWhiteSpace(department))
                return BadRequest("Department can not be blank");

            else if (department.Length < 3 || department.Length > 30)
                return BadRequest("Department should be between 3 and 30 characters.");

            DataTable dataTable = _teacherRepository.GetTeachersByDepartmentByTeacherName(department, teacherName);

            if (dataTable.Rows.Count > 0)
                return Ok(JsonConvert.SerializeObject(dataTable));
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetTeacherBySalaryRange/{minimumSalary}/{maximumSalary}")]
        public IActionResult GetTeacherBySalaryRange(int minimumSalary, int maximumSalary)
        {
            if (maximumSalary < minimumSalary)
                return BadRequest("Maximum salary cannot be less than minimum salary");

            DataTable dataTable = _teacherRepository.GetTeacherBySalaryRange(minimumSalary, maximumSalary);

            if (dataTable.Rows.Count > 0)
                return Ok(JsonConvert.SerializeObject(dataTable));
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetTeacherFullNameById/{TeacherId}")]
        public IActionResult GetTeacherFullNameById(int teacherId)
        {
            if (teacherId < 1)
                return BadRequest("TeacherId should be greater than 0");

            string teacherFullName = _teacherRepository.GetTeacherFullNameById(teacherId);
            return Ok(teacherFullName);
        }


        [HttpPost]
        [Route("TeacherRegister")]
        public IActionResult TeacherRegister([FromBody] TeacherDto teacher)
        {
            try
            {
                string errorMessage = validateTeacherRegisterOrUpdate(teacher);
                if (!string.IsNullOrEmpty(errorMessage))
                    return BadRequest(errorMessage);

                if (ModelState.IsValid)
                {
                    int id = _teacherRepository.Add(teacher);
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
        [Route("TeacherUpdate")]
        public IActionResult TeacherUpdate([FromBody] TeacherDto teacher)
        {
            try
            {
                string errorMessage = validateTeacherRegisterOrUpdate(teacher, true);
                if (!string.IsNullOrEmpty(errorMessage))
                    return BadRequest(errorMessage);

                if (ModelState.IsValid)
                {
                    _teacherRepository.Update(teacher);
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

        private string validateTeacherRegisterOrUpdate(TeacherDto teacher, bool isUpdate = false)
        {
            string errorMessage = "";

            teacher.FullName = teacher.FullName.Trim();
            teacher.Gender = teacher.Gender.Trim();
            teacher.SchoolName = teacher.SchoolName.Trim();
            teacher.Department = teacher.Department.Trim();

            if (isUpdate == true)
            {
                if (teacher.Id < 1)
                    errorMessage = "Id can not be less than 0";
            }

            if (string.IsNullOrWhiteSpace(teacher.FullName))
                errorMessage = "FullName can not be blank";

            else if (teacher.FullName.Length < 3 || teacher.FullName.Length > 30)
                errorMessage = "FullName should be between 3 and 30 characters.";

            else if (teacher.Age <= 18)
                errorMessage = "Invalid age, customer age should be above 18";

            else if (string.IsNullOrWhiteSpace(teacher.Department))
                errorMessage = "Department can not be blank";

            else if (string.IsNullOrWhiteSpace(teacher.SchoolName))
                errorMessage = "SchoolName can not be blank";

            else if (string.IsNullOrWhiteSpace(teacher.Gender))
                errorMessage = "Gender can not be blank";

            return errorMessage;
        }
    }
}