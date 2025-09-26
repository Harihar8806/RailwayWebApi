using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace RailwayWebApi.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? Token { get; set; }
    }
}
