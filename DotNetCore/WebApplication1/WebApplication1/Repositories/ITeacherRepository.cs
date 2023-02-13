using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public interface ITeacherRepository
    {
        public List<TeacherDto> GetAllTeachersAsList();
        public int GetTeachersCount();
        public TeacherDto GetTeacherDetailById(int teacherId);
        public List<TeacherDto> GetTeachersByDepartmentByTeacherName(string department, string teacherName);
        public List<TeacherDto> GetTeacherBySalaryRange(int minimumSalary, int maximumSalary);
        public string GetTeacherFullNameById(int teacherId);
        public int Add(TeacherDto teacher);
        public void Update(TeacherDto teacher);
    }
}
    