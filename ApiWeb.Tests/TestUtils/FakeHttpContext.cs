using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ApiWeb.Tests.TestUtils
{
    public static class FakeHttpContext
    {
        public static HttpContext CreateFakeContext(string userId)
        {
            var context = new DefaultHttpContext();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var principal = new ClaimsPrincipal(identity);

            context.User = principal;

            return context;
        }
    }
}
