using DemoAPIs.Entity.Data;
using DemoAPIs.Entity.Models;
using DemoAPIs.Entity.ViewModels.Index;
using DemoAPIs.Repository.Interface;
using DemoAPIs.Repository.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPIs.Repository.Repository
{
    public class IndexRepository : IIndexRepository
    {
        private readonly ApplicationDBContext _context;
        private readonly IConfiguration _config;
        public IndexRepository(ApplicationDBContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        #region Create User
        public void CreateUser(LoginModel model)
        {
            User user = new User();
            user.Email = model.email;
            user.Password = BCrypt.Net.BCrypt.HashPassword(model.password);
            _context.Add(user);
            _context.SaveChanges();
        }
        #endregion

        #region Check User Exists
        public bool UserExists(LoginModel model)
        {
            if (_context.Users.Any(e => e.Email == model.email))
            {
                string hashedPassword = _context.Users.FirstOrDefault(e => e.Email == model.email).Password;
                if (hashedPassword != null)
                {
                    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.password, hashedPassword);
                    return isPasswordValid;
                }
            }
            return false;
        }
        #endregion

        #region Generate JWT Token
        public string GenerateToken(string email)
        {
            if (_context.Users.Any(e => e.Email == email))
            {
                JwtToken jwt = new JwtToken();
                return jwt.GenerateJwtToken(_config, email);
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Validate JWT Token
        public bool ValidateToken(string token)
        {
            JwtToken jwt = new JwtToken();
            return jwt.ValidateToken(_config, token);
        }
        #endregion

        #region Generate Refresh Token
        public RefreshToken GenerateRefreshToken(string email)
        {
            if (_context.Users.Any(e => e.Email == email))
            {
                RefreshToken token = new RefreshToken
                {
                    refreshToken = Guid.NewGuid().ToString(),
                    ExpiresAt = DateTime.UtcNow.AddDays(Convert.ToDouble(_config["Jwt:RefreshTokenExpirationDays"]))
                };
                User user = _context.Users.FirstOrDefault(e => e.Email == email);
                user.Refreshtoken = token.refreshToken;
                user.Expiresat = token.ExpiresAt;
                _context.Update(user);
                _context.SaveChanges();
                return token;
            }
            else
            {
                RefreshToken token = new RefreshToken();
                return token;
            }
        }
        #endregion

        #region Validate Refresh Token
        public bool ValidateRefreshToken(string refreshToken)
        {
            //2nd condition is :- refreshToken's ExpiresAt < DateTime.UtcNow, ExpiresAt will get from the DB from token.
            DateTime? expiryDate = _context.Users.FirstOrDefault(e => e.Refreshtoken == refreshToken).Expiresat;
            if (refreshToken != null && expiryDate != null && expiryDate > DateTime.UtcNow)
            {
                return true;
            }
            return false;
        }
        #endregion

        public string ValidateTokenWithoutLifeTime(string accessToken)
        {
            JwtToken jwt = new JwtToken();
            JwtSecurityToken token = jwt.ValidateTokenWithoutLifeTime(_config, accessToken);
            string email = null;
            email = token.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Email).Value;
            return email;
        }

        public bool EmailExists(string email)
        {
            return _context.Users.Any(e => e.Email == email);
        }

        public ForgotToken GenerateForgotToken(ForgotPasswordViewModel model)
        {
            if (_context.Users.Any(e => e.Email == model.email))
            {
                ForgotToken token = new ForgotToken
                {
                    Token = Guid.NewGuid().ToString(),
                    ForgotTokenExpires = DateTime.UtcNow.AddDays(2)
                };
                User user = _context.Users.FirstOrDefault(e => e.Email == model.email);
                user.Forgotpasswordtoken = token.Token;
                user.Forgottokenexpiry = token.ForgotTokenExpires;
                _context.Update(user);
                _context.SaveChanges();
                return token;
            }
            else
            {
                ForgotToken token = new ForgotToken();
                return token;
            }
        }

        public bool ValidateForgotToken(string token, string email)
        {
            //2nd condition is :- token's Forgottokenexpiry < DateTime.UtcNow, Forgottokenexpiry will get from the DB from token.
            if (_context.Users.FirstOrDefault(e => e.Email == email).Forgotpasswordtoken != null)
            {
                DateTime? expiryDate = _context.Users.FirstOrDefault(e => e.Forgotpasswordtoken == token).Forgottokenexpiry;
                if (token != null && expiryDate != null && expiryDate > DateTime.UtcNow)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ResetPassword(ResetPasswordViewModel model)
        {
            User user = _context.Users.FirstOrDefault(e => e.Email == model.Email);
            if (user.Forgotpasswordtoken == model.Token)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.Password);
                user.Forgotpasswordtoken = null;
                user.Forgottokenexpiry = null;
                _context.Update(user);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
