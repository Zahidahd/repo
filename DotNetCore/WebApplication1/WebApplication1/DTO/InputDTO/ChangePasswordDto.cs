namespace WebApplication1.DTO.InputDTO
{
    public class ChangePasswordDto
    {
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ReEnterPassword { get; set; }
    }
}
    