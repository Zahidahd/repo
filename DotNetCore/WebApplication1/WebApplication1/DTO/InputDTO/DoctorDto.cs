using WebApplication1.Enums;
using static WebApplication1.Enums.GenderTypes;

namespace WebApplication1.DTO.InputDTO
{
    public class DoctorDto
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public GenderType Gender { get; set; }

        public string Department { get; set; }

        public string City { get; set; }
    }
}
