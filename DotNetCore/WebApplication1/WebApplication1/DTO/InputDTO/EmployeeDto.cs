using WebApplication1.Enums;
using static WebApplication1.Enums.GenderTypes;

namespace WebApplication1.DTO.InputDTO
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public GenderTypes Gender { get; set; }
        public string DateOfJoining { get; set; }
        public int Salary { get; set; }
    }
}
