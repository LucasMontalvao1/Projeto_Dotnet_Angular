using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace ApiWeb.Tests.TestUtils
{
    public static class FakeHttpContext
    {
        private const string DEFAULT_AUTH_SCHEME = "TestAuthScheme";
        private const string DEFAULT_ROLE = "User";

        /// <summary>
        /// Cria um contexto HTTP falso com claims básicas de usuário
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="role">Papel do usuário (default: User)</param>
        /// <param name="authScheme">Esquema de autenticação (default: TestAuthScheme)</param>
        /// <returns>HttpContext com as claims configuradas</returns>
        public static HttpContext CreateFakeContext(
            string userId,
            string role = DEFAULT_ROLE,
            string authScheme = DEFAULT_AUTH_SCHEME)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("UserId não pode ser nulo ou vazio", nameof(userId));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            return CreateFakeContextWithClaims(claims, authScheme);
        }

        /// <summary>
        /// Cria um contexto HTTP falso com claims personalizadas
        /// </summary>
        /// <param name="claims">Lista de claims</param>
        /// <param name="authScheme">Esquema de autenticação</param>
        /// <returns>HttpContext com as claims configuradas</returns>
        public static HttpContext CreateFakeContextWithClaims(
            IEnumerable<Claim> claims,
            string authScheme = DEFAULT_AUTH_SCHEME)
        {
            if (claims == null || !claims.Any())
                throw new ArgumentException("Claims não podem ser nulas ou vazias", nameof(claims));

            var context = new DefaultHttpContext();
            var identity = new ClaimsIdentity(claims, authScheme);
            var principal = new ClaimsPrincipal(identity);
            context.User = principal;

            // Simula headers comuns
            context.Request.Headers["User-Agent"] = "TestAgent";
            context.Request.Headers["Accept"] = "application/json";

            return context;
        }

        /// <summary>
        /// Cria um contexto HTTP falso para usuário anônimo
        /// </summary>
        /// <returns>HttpContext sem autenticação</returns>
        public static HttpContext CreateAnonymousContext()
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["User-Agent"] = "TestAgent";
            context.Request.Headers["Accept"] = "application/json";
            return context;
        }

        /// <summary>
        /// Cria um contexto HTTP falso com múltiplos roles
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="roles">Lista de roles</param>
        /// <returns>HttpContext com múltiplos roles</returns>
        public static HttpContext CreateFakeContextWithRoles(
            string userId,
            IEnumerable<string> roles)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("UserId não pode ser nulo ou vazio", nameof(userId));

            if (roles == null || !roles.Any())
                throw new ArgumentException("Roles não podem ser nulos ou vazios", nameof(roles));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            return CreateFakeContextWithClaims(claims);
        }

        /// <summary>
        /// Cria um contexto HTTP falso com claims específicas para testes de API
        /// </summary>
        /// <param name="userId">ID do usuário</param>
        /// <param name="role">Role do usuário</param>
        /// <param name="additionalClaims">Claims adicionais</param>
        /// <returns>HttpContext configurado para testes de API</returns>
        public static HttpContext CreateApiTestContext(
            string userId,
            string role = DEFAULT_ROLE,
            IDictionary<string, string> additionalClaims = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            if (additionalClaims != null)
            {
                claims.AddRange(additionalClaims.Select(c => new Claim(c.Key, c.Value)));
            }

            var context = CreateFakeContextWithClaims(claims);

            // Configurações específicas para API
            context.Request.Headers["Accept"] = "application/json";
            context.Request.Headers["Content-Type"] = "application/json";
            context.Request.Headers["Authorization"] = "Bearer test-token";

            return context;
        }
    }
}