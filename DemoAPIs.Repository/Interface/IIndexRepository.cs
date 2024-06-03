using DemoAPIs.Entity.ViewModels.Index;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoAPIs.Repository.Interface
{
    public interface IIndexRepository
    {
        void CreateUser(LoginModel model);
        bool UserExists(LoginModel model);
        string GenerateToken(string email);
        bool ValidateToken(string token);
        RefreshToken GenerateRefreshToken(string email);
        bool ValidateRefreshToken(string refreshToken);
        string ValidateTokenWithoutLifeTime(string accessToken);
        bool EmailExists(string email);
        ForgotToken GenerateForgotToken(ForgotPasswordViewModel model);
        bool ValidateForgotToken(string token,string email);
        bool ResetPassword(ResetPasswordViewModel model);
    }
}
