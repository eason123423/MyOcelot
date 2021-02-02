using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyOcelot.Authentication.JWT
{
    /// <summary>
    /// token创建
    /// </summary>
    public class TokenBuilder
    {
        private JwtTokenOptions _tokenOptions = null;
        public TokenBuilder(IOptions<JwtTokenOptions> tokenOptions)
        {
            this._tokenOptions = tokenOptions.Value;
        }
        /// <summary>
        /// 创建加密JwtToken
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string CreateJwtToken(User user)
        {
            var claimList = this.CreateClaimList(user);
            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: this._tokenOptions.Issuer
                , audience: this._tokenOptions.Audience
                , claims: claimList
                //, notBefore: utcNow
                , expires: DateTime.Now.AddMinutes(1)
                , signingCredentials: new SigningCredentials(new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(this._tokenOptions.SigningKey)), SecurityAlgorithms.HmacSha256)
            );
            string jwtToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            //加密jwtToken
            jwtToken = AESCryptoHelper.Encrypt(jwtToken);
            return jwtToken;
        }

        /// <summary>
        /// 创建包含用户信息的CalimList
        /// </summary>
        /// <param name="authUser"></param>
        /// <returns></returns>
        private List<Claim> CreateClaimList(User authUser)
        {
            //身份单元项项集合
            List<Claim> claimList = new List<Claim>()
                    {
                        new Claim(type: ClaimTypes.Email, value: authUser.Email), //身份单元项
                        new Claim(type: ClaimTypes.Name, value: authUser.Name),
                        new Claim(type: ClaimTypes.NameIdentifier, value: authUser.UserID.ToString()),
                        new Claim(type: ClaimTypes.Role, value: authUser.Role ?? string.Empty)
                    };
            return claimList;
        }
    }
}
