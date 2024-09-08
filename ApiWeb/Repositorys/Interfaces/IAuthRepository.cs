using ApiWeb.Models;

namespace ApiWeb.Repositorys.Interfaces
{
    public interface IAuthRepository
    {
        User ValidarUsuario(User user);
    }
}
