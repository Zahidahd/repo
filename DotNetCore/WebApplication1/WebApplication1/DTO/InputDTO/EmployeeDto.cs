using WebApplication1.Enums;
using static WebApplication1.Enums.GenderTypes;

namespace WebApplication1.DTO.InputDTO
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public GenderTypes Gender { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string DateOfJoining { get; set; }
        public decimal Salary { get; set; }
        public DateTime LastFailedLoginDate { get; set; }
        public DateTime LastSuccessfulLoginDate { get; set; }
        public int LoginFailedCount { get; set; }
        public bool IsLocked { get; set; }
    }
}   
