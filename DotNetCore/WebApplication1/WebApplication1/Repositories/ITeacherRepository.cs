using System.Data;
using WebApplication1.DTO.InputDTO;

namespace WebApplication1.Repositories
{
    public interface ITeacherRepository
    {
        public DataTable GetAllTeachers();
        public int GetTeachersCount();
        public DataTable GetTeacherDetailById(int teacherId);
        public DataTable GetTeachersByDepartmentByTeacherName(string department, string teacherName);
        public DataTable GetTeacherBySalaryRange(int minimumSalary, int maximumSalary);
        public string GetTeacherFullNameById(int teacherId);
        public int Add(TeacherDto teacher);
        public void Update(TeacherDto teacher);
    }
}
