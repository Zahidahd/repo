using WebApplication1.Enums;
using static WebApplication1.Enums.GenderTypes;

namespace WebApplication1.DTO.InputDTO
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public GenderTypes Gender { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public byte[] HashValuePassword { get; set; }
        public string? MobileNumber { get; set; }     
        public DateTime? LastFailedLoginDate { get; set; }    
        public DateTime? LastSuccessfulLoginDate { get; set; }    
        public int LoginFailedCount { get; set  ; }    
        public bool IsLocked { get; set; }    
        public string Country { get; set; }

        public CustomerDto()
        {

        }

        public CustomerDto(int id, string fullName, GenderTypes gender, int age, string email, string password, 
            string mobileNumber, string country)
        {
            Id = id;
            FullName = fullName;
            Gender = gender;
            Age = age;
            Email = email;
            Password = password;
            MobileNumber = mobileNumber;
            Country = country;
        }
    }
}           