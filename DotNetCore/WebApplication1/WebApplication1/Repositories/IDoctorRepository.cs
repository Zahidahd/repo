using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public interface IDoctorRepository
    {
        public List<DoctorDto> GetAllDoctorsAsList();
        public int GetDoctorsCount();
        public DoctorDto GetDoctorDetailById(int doctorId);
        public List<DoctorDto> GetDoctorsByDepartmentByDoctorName(string department, string doctorName);
        public List<DoctorDto> GetDoctorsNameListByDepartment(string department);
        public string GetDoctorFullNameByDoctorId(int doctorId);
        public int Add(DoctorDto doctor);
        public void Update(DoctorDto doctor);
    }
}
