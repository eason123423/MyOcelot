using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyIdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };


        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new ApiScope("password_scope1"),
                 new ApiScope("agentservice"),
                  new ApiScope("clientservice"),
                  new ApiScope("productservice")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {

                new ApiResource("api1","api1")
                {
                    Scopes={ "password_scope1" },
                    UserClaims={JwtClaimTypes.Role},  //添加Cliam 角色类型
                    ApiSecrets={new Secret("apipwd".Sha256())}
                },
                 new ApiResource("clientservice", "CAS Client Service"),
                new ApiResource("productservice", "CAS Product Service"),
                new ApiResource("agentservice", "CAS Agent Service")
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {


                 new Client
                {
                    ClientId = "password_client",
                    ClientName = "Resource Owner Password",

                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,//密码方式
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },//秘钥
                   AllowAccessTokensViaBrowser = true,// 
                   //AllowedCorsOrigins = { "http://192.167.200.8:8080" },允许跨域
                    AccessTokenLifetime = 60*60,//一小时      appSettings.AdminUI.AccessTokenLifetime, //2小时 = 3600 * 2
                    AllowedScopes = { "password_scope1" }//授权范围
                },
                 new Client
                {
                    ClientId = "cas.mvc.client.implicit",
                    ClientName = "CAS MVC Web App Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = { $"http://localhost:49247/signin-oidc" },//登录成功跳转地址
                    PostLogoutRedirectUris = { $"http://localhost:49247/signout-callback-oidc" },//退出跳转地址
                    AllowedScopes = new [] {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "agentservice", "clientservice", "productservice","password_scope1"
                    },

                    AllowAccessTokensViaBrowser = true // can return access_token to this client
                },
            };
    }
}
