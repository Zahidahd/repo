using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public interface IDoctorRepository
    {
        public DataTable GetAllDoctors();
        public int GetDoctorsCount();
        public DataTable GetDoctorDetailById(int doctorId);
        public DataTable GetDoctorsByDepartmentByDoctorName(string department, string doctorName);
        public DataTable GetDoctorsNameListByDepartment(string department);
        public string GetDoctorFullNameByDoctorId(int doctorId);
        public int Add(DoctorDto doctor);
        public void Update(DoctorDto doctor);
    }
}
