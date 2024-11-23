using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace ApiWeb.Tests.TestUtils
{
    public static class FakeHttpContext
    {
        public static HttpContext CreateFakeContext(string userId, string role = "User")
        {
            var context = new DefaultHttpContext();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            context.User = principal;

            return context;
        }

        public static HttpContext CreateFakeContextWithClaims(IEnumerable<Claim> claims)
        {
            var context = new DefaultHttpContext();
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);
            context.User = principal;

            return context;
        }

        public static HttpContext CreateAnonymousContext()
        {
            return new DefaultHttpContext();
        }
    }
}