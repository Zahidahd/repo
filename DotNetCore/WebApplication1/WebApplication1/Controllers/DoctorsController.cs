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
using System.Numerics;
using System.Text.RegularExpressions;
using static WebApplication1.Enums.GenderTypes;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        IDoctorRepository _doctorRepository;

        public DoctorsController(IDoctorRepository doctorRepository)
        {
            _doctorRepository = doctorRepository;
        }

        [HttpGet]
        [Route("GetAllDoctors")]
        public IActionResult GetAllDoctors()
        {
            DataTable dataTable = _doctorRepository.GetAllDoctors();

            if (dataTable.Rows.Count > 0)
                return Ok(JsonConvert.SerializeObject(dataTable));
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetDoctorsCount")]
        public IActionResult GetDoctorsCount()
        {
            int doctorCount = _doctorRepository.GetDoctorsCount();
            return Ok(doctorCount);
        }

        [HttpGet]
        [Route("GetDoctorDetailById/{doctorId}")]
        public IActionResult GetDoctorDetailById(int doctorId)
        {
            if (doctorId < 1)
                return BadRequest("Doctor Id should be greater than 0");

            DataTable dataTable = _doctorRepository.GetDoctorDetailById(doctorId);

            if (dataTable.Rows.Count > 0)
                return Ok(JsonConvert.SerializeObject(dataTable));
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetDoctorsByDepartmentByDoctorName/{department}/{doctorName}")]
        public IActionResult GetDoctorsByDepartmentByDoctorName(string department, string doctorName)
        {
            doctorName = doctorName.Trim();
            department = department.Trim();

            if (string.IsNullOrWhiteSpace(doctorName))
                return BadRequest("Doctor name cannot be blank");

            else if (doctorName.Length < 3 || doctorName.Length > 30)
                return BadRequest("DoctorName should be between 3 and 30 characters.");

            else if (department.Length < 3 || department.Length > 30)
                return BadRequest("Department should be between 3 and 30 characters.");

            DataTable dataTable = _doctorRepository.GetDoctorsByDepartmentByDoctorName(department, doctorName);

            if (dataTable.Rows.Count > 0)
                return Ok(JsonConvert.SerializeObject(dataTable));
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetDoctorsNameListByDepartment/{department}")]
        public IActionResult GetDoctorsNameListByDepartment(string department)
        {
            if (string.IsNullOrWhiteSpace(department))
                return BadRequest("Department cannot be blank");

            else if (department.Length < 3 || department.Length > 30)
                return BadRequest("Department should be between 3 and 30 characters.");

            DataTable dataTable = _doctorRepository.GetDoctorsNameListByDepartment(department);

            if (dataTable.Rows.Count > 0)
                return Ok(JsonConvert.SerializeObject(dataTable));
            else
                return NotFound();
        }

        [HttpGet]
        [Route("GetDoctorFullNameByDoctorId/{doctorId}")]
        public IActionResult GetDoctorFullNameByDoctorId(int doctorId)
        {
            if (doctorId < 1)
                return BadRequest("Doctor Id cannot be less than 1");

            string subStringDoctorFullName = _doctorRepository.GetDoctorFullNameByDoctorId(doctorId);
            return Ok(subStringDoctorFullName);
        }

        [HttpPost]
        [Route("DoctorRegister")]
        public IActionResult DoctorRegister([FromBody] DoctorDto doctor)
        {
            try
            {
                string errorMessage = validateDoctorRegisterOrUpdate(doctor);
                if (!string.IsNullOrEmpty(errorMessage))
                    return BadRequest(errorMessage);

                if (ModelState.IsValid)
                {
                    int doctorId = _doctorRepository.Add(doctor);
                    return Ok(doctorId);
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
        [Route("DoctorUpdate")]
        public IActionResult DoctorUpdate([FromBody] DoctorDto doctor)
        {
            try
            {
                string errorMessage = validateDoctorRegisterOrUpdate(doctor, true);
                if (!string.IsNullOrEmpty(errorMessage))
                    return BadRequest(errorMessage);

                if (ModelState.IsValid)
                {
                    _doctorRepository.Update(doctor);
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

        private string validateDoctorRegisterOrUpdate(DoctorDto doctor, bool isUpdate = false)
        {
            string errorMessage = "";

            doctor.FullName = doctor.FullName.Trim();
            doctor.Department = doctor.Department.Trim();

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(doctor.Email);

            if (isUpdate == true)
            {
                if (doctor.Id < 1)
                    errorMessage = "Id can not be less than 0";
            }

            if (!match.Success)
                errorMessage = "Email is invalid";

            else if (string.IsNullOrWhiteSpace(doctor.FullName))
                errorMessage = "FullName cannot be blank";

            else if (doctor.FullName.Length < 3 || doctor.FullName.Length > 30)
                errorMessage = "FullName should be between 3 and 30 characters";

            else if (string.IsNullOrWhiteSpace(doctor.Department))
                errorMessage = "Department cannot be blank";

            else if (!Enum.IsDefined(typeof(GenderType), doctor.Gender))
                errorMessage = "Invalid Gender";

            return errorMessage;
        }
    }
}