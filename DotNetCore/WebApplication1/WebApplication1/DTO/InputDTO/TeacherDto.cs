using WebApplication1.Enums;
using static WebApplication1.Enums.GenderTypes;

namespace WebApplication1.DTO.InputDTO
{
    public class TeacherDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
        public GenderTypes Gender { get; set; }
        public string SchoolName { get; set; }
        public string Department { get; set; }
        public int Salary { get; set; }
    }
}
