using ApiWeb.Models;
using ApiWeb.Repositorys.Interfaces;
using ApiWeb.Services.Interfaces;

namespace ApiWeb.Services
{
    public class TokenService : ITokenService
    {
        private readonly ITokenRepository _tokenRepository;

        public TokenService(ITokenRepository tokenRepository)
        {
            _tokenRepository = tokenRepository;
        }

        public string GenerateToken(User user)
        {
            return _tokenRepository.CreateToken(user);
        }

        public bool ValidateToken(string token)
        {
            return _tokenRepository.IsValidToken(token);
        }
    }
}
