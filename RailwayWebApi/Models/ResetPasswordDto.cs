namespace RailwayWebApi.Models
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string NewPassword { get; set; }
    }
}
