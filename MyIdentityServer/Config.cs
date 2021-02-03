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
                  new ApiScope("productservice"),
                   new ApiScope("api1")
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
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // client credentials flow client
                new Client
                {
                    ClientId = "client",
                    ClientName = "Client Credentials Client",
                    AllowOfflineAccess = true,  //是否可以离线访问，refresh
                    AllowedGrantTypes = GrantTypes.ClientCredentials,   //不代表任何用户
                    ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

                    AllowedScopes = { "api1", IdentityServerConstants.StandardScopes.OfflineAccess }
                },
                //pwd client
                new Client
                {
                    ClientId = "wpf client",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets=
                    {
                        new Secret("wpf secret".Sha256())
                    },
                    AllowOfflineAccess = true,  //是否可以离线访问，refresh
                    AllowedScopes={ "api1", "openid", "profile", IdentityServerConstants.StandardScopes.OfflineAccess }//
                },
                // MVC client using hybrid flow
                new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",

                    AllowedGrantTypes = GrantTypes.Code,
                    ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    FrontChannelLogoutUri = "http://localhost:5002/signout-oidc",
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },
                    AccessTokenLifetime = 60,
                    AlwaysIncludeUserClaimsInIdToken = true, //返回token时候同时返回用户Claims
                    AllowOfflineAccess = true,  //是否可以离线访问，refresh
                    AllowedScopes = { "openid", "profile", "api1", "email", "address", "phone" }
                },

                //// SPA client using implicit flow
                //new Client
                //{
                //    ClientId = "spa",
                //    ClientName = "SPA Client",
                //    ClientUri = "http://identityserver.io",

                //    AllowedGrantTypes = GrantTypes.Implicit,
                //    AllowAccessTokensViaBrowser = true,

                //    RedirectUris =
                //    {
                //        "http://localhost:5002/index.html",
                //        "http://localhost:5002/callback.html",
                //        "http://localhost:5002/silent.html",
                //        "http://localhost:5002/popup.html",
                //    },

                //    PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
                //    AllowedCorsOrigins = { "http://localhost:5002" },

                //    AllowedScopes = { "openid", "profile", "api1" }
                //},
                // JavaScript Client
                new Client
                {
                    ClientId = "js",
                    ClientName = "JavaScript Client",
                    ClientUri = "http://localhost:5003",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris =           { "http://localhost:5003/callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:5003/index.html" },
                    AllowedCorsOrigins =     { "http://127.0.0.1:5003" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "api1"
                    }
                },
                // mvc hybrid client
                new Client
                {
                    ClientId = "hybrid client",
                    ClientName = "mvc hybrid Client",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    AccessTokenType = AccessTokenType.Reference,

                    ClientSecrets = {new Secret("hybrid client".Sha256())},

                    RedirectUris =           { "http://localhost:5004/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5004/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true, // user claims 放到id token里面去
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Phone,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Address,
                        "api1",
                        "roles",
                        "locations"
                    }
                },
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

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResources.Address(),
                new IdentityResources.Phone(),
                new IdentityResource("roles","角色",new List<string>{ "role"}),
                new IdentityResource("locations","地点",new List<string>{ "location"}),
            };
        }

        public static IEnumerable<ApiResource> GetApis()
        {
            return new ApiResource[]
            {
                new ApiResource("api2", "My API #1",new List<string>{ "location" }),
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
        }
    }
}
