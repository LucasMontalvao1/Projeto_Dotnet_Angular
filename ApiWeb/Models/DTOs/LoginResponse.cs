namespace ApiWeb.Models.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; }
        public UserDto User { get; set; }
    }
}
