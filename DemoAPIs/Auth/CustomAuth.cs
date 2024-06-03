using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DemoAPIs.Repository.Interface;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DemoAPIs.Auth
{
    public class CustomAuth : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var _indexRepository = context.HttpContext.RequestServices.GetService<IIndexRepository>();

            var authorizationHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authorizationHeader != null && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Substring("Bearer ".Length).Trim();
                if (token != null)
                {
                    JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(token);
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(jwtSecurityToken.Claims, "jwt"));
                    if (!_indexRepository.ValidateToken(token))
                    {
                        string refreshToken = context.HttpContext.Request.Headers["RefreshToken"];
                        /*here i have to check for the referesh token 
                         refresh token expiresat < current than refresh-token request should be made
                         */
                        if (refreshToken != null)
                        {
                            if (_indexRepository.ValidateRefreshToken(refreshToken))
                            {
                                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Index", action = "ValidRefresh" }));
                                return;
                            }
                            else
                            {
                                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Index", action = "InvalidToken" }));
                                return;
                            }
                        }
                        else
                        {
                            context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Index", action = "NullToken" }));
                            return;
                        }
                    }
                    else
                    {
                        context.HttpContext.User = claimsPrincipal;
                    }
                }
                else
                {
                    context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Index", action = "NullToken" }));
                    return;
                }
            }
            else
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Index", action = "InvalidHeader" }));
                return;
            }

        }

    }
}
