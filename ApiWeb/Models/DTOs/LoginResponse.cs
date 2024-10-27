namespace ApiWeb.Models.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserResponseDto User { get; set; } // Mude de UserDto para UserResponseDto
    }
}
