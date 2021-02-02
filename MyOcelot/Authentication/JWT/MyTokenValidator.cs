using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyOcelot.Authentication.JWT
{
    public class MyTokenValidator : ISecurityTokenValidator
    {
        bool ISecurityTokenValidator.CanValidateToken => true;

        int ISecurityTokenValidator.MaximumTokenSizeInBytes { get; set; }

        bool ISecurityTokenValidator.CanReadToken(string securityToken)
        {
            return true;
        }

        //验证token
        ClaimsPrincipal ISecurityTokenValidator.ValidateToken(string securityToken, TokenValidationParameters validationParameters, out SecurityToken validatedToken)
        {
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(securityToken, validationParameters, out validatedToken);
            return principal;
        }
    }
}
