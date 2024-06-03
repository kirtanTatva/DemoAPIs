
using DemoAPIs.Entity.ViewModels.Index;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPIs.Repository.Utilities
{
    public class JwtToken
    {
        public string GenerateJwtToken(IConfiguration _config, string email)
        {
            var claims = new[] {
                new Claim(ClaimTypes.Email, email),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_config["Jwt:TokenValidityInMinutes"]));

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool ValidateToken(IConfiguration _config, string token)
        {
            JwtSecurityToken jwtSecurityToken = null;

            if (token == null)
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidAudience = _config["Jwt:Audience"],
                    ValidIssuer = _config["Jwt:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                jwtSecurityToken = (JwtSecurityToken)validatedToken;

                if (jwtSecurityToken != null)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public JwtSecurityToken ValidateTokenWithoutLifeTime(IConfiguration _config, string accessToken)
        {
            JwtSecurityToken jwtSecurityToken = null;
            if (accessToken != null)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
                try
                {
                    tokenHandler.ValidateToken(accessToken, new TokenValidationParameters
                    {
                        ValidateLifetime = false,
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidAudience = _config["Jwt:Audience"],
                        ValidIssuer = _config["Jwt:Issuer"],
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ClockSkew = TimeSpan.Zero

                    }, out SecurityToken validatedToken);

                    jwtSecurityToken = (JwtSecurityToken)validatedToken;

                    return jwtSecurityToken;
                }
                catch
                {
                    return jwtSecurityToken;
                }
            }
            return jwtSecurityToken;
        }
    }
}
