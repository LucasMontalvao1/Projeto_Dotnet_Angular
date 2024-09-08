using ApiWeb.Models;

namespace ApiWeb.Repositorys.Interfaces
{
    public interface ITokenRepository
    {
        string CreateToken(User user);
        bool IsValidToken(string token);
    }
}
