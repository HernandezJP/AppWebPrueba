using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppWebPrueba.Services
{
    public class JwtTokenService
    {
        public ClaimsPrincipal CreatePrincipalFromToken(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,
                    token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value
                    ?? token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value
                    ?? "")
            };

            var roles = token.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type == "role").Select(c => c.Value);
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var identity = new ClaimsIdentity(claims, "cookie");
            return new ClaimsPrincipal(identity);
        }
    }
}
