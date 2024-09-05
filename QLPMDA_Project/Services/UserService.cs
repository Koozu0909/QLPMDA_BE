using Microsoft.IdentityModel.Tokens;
using QLPMDA_Project.Extensions;
using QLPMDA_Project.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace TMS.API.Services
{
    public class UserService
    {
        private const int MAX_LOGIN = 10;
        public readonly IHttpContextAccessor Context;
        private readonly QLPMDAContext db;
        private readonly IConfiguration _configuration;
        public int UserId { get; set; }

        public UserService(IHttpContextAccessor httpContextAccessor, QLPMDAContext db, IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            Context = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            if (Context?.HttpContext is null)
            {
                UserId = Utils.SystemId;
                return;
            }
            var claims = Context.HttpContext.User.Claims;
            UserId = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value?.TryParseInt() ?? 0;
        }

        public string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public string GenerateRandomToken(int? maxLength = 32)
        {
            var builder = new StringBuilder();
            var random = new Random();
            char ch;
            for (int i = 0; i < maxLength; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public string Generate(Users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role),
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }

        public Users? GetCurrentUser()
        {
            var identity = Context.HttpContext?.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new Users
                {
                    Id = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value?.TryParseInt() ?? 0,
                    UserName = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value,
                    Email = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Email)?.Value,
                    Role = userClaims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value,
                };
            }
            return null;
        }
    }
}
