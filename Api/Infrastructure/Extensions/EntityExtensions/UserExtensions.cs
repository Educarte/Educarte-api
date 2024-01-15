using Api.Infrastructure.Options;
using Core;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api.Infrastructure.Extensions.EntityExtensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Generate a user auth token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authTokenOptions"></param>
        /// <returns></returns>
        public static JwtSecurityToken GenerateAuthToken(this User user, AuthTokenOptions authTokenOptions)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", user.Profile.ToString()),
                new Claim("profile", user.Profile.ToString())
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authTokenOptions.Key)), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(authTokenOptions.Issuer,
                                             authTokenOptions.Audience,
                                             claims,
                                             expires: DateTime.Now.AddMinutes(authTokenOptions.TokenDuration),
                                             signingCredentials: creds);

            return token;
        }
    }
}

