using DemoAPIs.Entity.ViewModels.Index;
using DemoAPIs.Repository.Interface;
using DemoAPIs.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DemoAPIs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IndexController : ControllerBase
    {
        private readonly IIndexRepository _indexRepository;
        private readonly IConfiguration _config;
        public IndexController(IIndexRepository indexRepository, IConfiguration config)
        {
            _indexRepository = indexRepository;
            _config = config;
        }

        #region Registration
        [Route("Registration")]
        [HttpPost]
        public ActionResult Registration(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (!_indexRepository.UserExists(model))
                {
                    _indexRepository.CreateUser(model);
                    return Ok("User created successfully");
                }
                else
                {
                    return Unauthorized("User Exists");
                }
            }
            else
            {
                return Unauthorized("Email or Password is null");
            }
        }
        #endregion

        #region Login
        [Route("Login")]
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                if (_indexRepository.UserExists(model))
                {
                    string token = _indexRepository.GenerateToken(model.email);
                    if (token != null)
                    {
                        RefreshToken refreshtoken = _indexRepository.GenerateRefreshToken(model.email);
                        return Ok(new { token, refreshtoken });
                    }
                    else
                    {
                        return Unauthorized("token not generated because email not exists");
                    }
                }
                else
                {
                    return Unauthorized("Incorrect Password");
                }
            }
            else
            {
                return Unauthorized("Email or Password is null");
            }
        }
        #endregion

        #region Null Token Msg
        [Route("NullToken")]
        [HttpGet]
        public ActionResult NullToken()
        {
            return Ok("Token is null");
        }
        #endregion

        #region Valid Refresh Token Msg
        [Route("ValidRefresh")]
        [HttpGet]
        public ActionResult ValidRefresh()
        {
            return Ok("Refresh Token is Valid now you can send RefreshToken Request");
        }
        #endregion

        #region InValid Token Msg
        [Route("InvalidToken")]
        [HttpGet]
        public ActionResult InvalidToken()
        {
            return Ok("Access and Refresh Token invalid");
        }
        #endregion

        #region InValid Header
        [Route("InvalidHeader")]
        [HttpGet]
        public ActionResult InvalidHeader()
        {
            return Ok("Header is null or invalid");
        }
        #endregion

        #region Refresh Token Generation
        [Route("RefreshToken")]
        [HttpPost]
        public ActionResult RefreshToken(AccessRefreshToken tokens)
        {
            if (tokens.AccessToken != null)
            {
                string email = _indexRepository.ValidateTokenWithoutLifeTime(tokens.AccessToken);
                if (email != null)
                {
                    //generate new tokens
                    string token = _indexRepository.GenerateToken(email);
                    if (token != null)
                    {
                        RefreshToken refreshtoken = _indexRepository.GenerateRefreshToken(email);
                        return Ok(new { token, refreshtoken });
                    }
                    else
                    {
                        return Unauthorized("token not generated because email not exists");
                    }
                }
                else
                {
                    return Unauthorized("Access Token is InValid");
                }
            }
            else
            {
                return Unauthorized("Access Token is null");
            }
        }
        #endregion

        #region ForgotPassword
        [Route("ForgotPassword")]
        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (_indexRepository.EmailExists(model.email))
                {
                    ForgotToken token = _indexRepository.GenerateForgotToken(model);
                    if (token != null)
                    {
                        string from = _config["Outlook:Email"];
                        string to = model.email;
                        string credential = _config["Outlook:Credential"];
                        string subject = "Reset Password Link";
                        string body = Url.Action("ResetPassword", "Index", new { email = model.email, token = token.Token }, Request.Scheme);
                        EmailService email = new EmailService();
                        email.SendMail(from, to, credential, subject, body, null);
                        return Ok(body);
                    }
                    else
                    {
                        return Unauthorized("Forgot Token not generated");
                    }
                }
                else
                {
                    return Unauthorized("Incorrect Email");
                }
            }
            else
            {
                return Unauthorized("Email is null");
            }
        }
        #endregion

        #region ResetPassword for View
        //This action method is for the view
        [HttpGet]
        public IActionResult ResetPassword(string Token, string Email)
        {
            if (Token == null || Email == null)
            {
                return Unauthorized("Token or email is null");
            }
            else
            {
                if (_indexRepository.ValidateForgotToken(Token,Email))
                {
                    return Ok("Password can be reset, reset page will loads");
                }
                else
                {
                    return Unauthorized("Forgot Token is not valid or expired");
                }
            }
        }
        #endregion

        #region Reset Password Operation
        [Route("ResetPassword")]
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                if (_indexRepository.EmailExists(model.Email))
                {
                    if (_indexRepository.ResetPassword(model))
                    {
                        return Ok("Reset Password Successfully");
                    }
                    else
                    {
                        return Unauthorized("Password not reset");
                    }
                }
                else
                {
                    return Unauthorized("Email not exist");
                }
            }
            else
            {
                return BadRequest();
            }
        }
        #endregion
    }
}
