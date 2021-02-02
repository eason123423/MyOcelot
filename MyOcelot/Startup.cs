using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MMLib.Ocelot.Provider.AppConfiguration;
using MyOcelot.Authentication.JWT;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MyOcelot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot().AddAppConfiguration();
            services.AddSwaggerForOcelot(Configuration);

            services.AddControllers();

            //注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TestSwaggerApi",
                    Description = "基于.NET Core 3.1 的测试实例",
                    Contact = new OpenApiContact
                    {
                        Name = "qxh",
                        Email = "2269667628@qq.com"
                    },
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Description = "在下框中输入请求头中需要添加Jwt授权Token：Bearer Token",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });

                #region swagger 用 Jwt验证
                //开启权限小锁
                c.OperationFilter<AddResponseHeadersFilter>();
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

                //在header中添加token，传递到后台
                c.OperationFilter<SecurityRequirementsOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Description = "JWT授权(数据将在请求头中进行传递)直接在下面框中输入Bearer {token}(注意两者之间是一个空格) \"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                    Type = SecuritySchemeType.ApiKey
                });
                #endregion

                // 为 Swagger JSON and UI设置xml文档注释路径
                var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                var xmlPath = Path.Combine(basePath, "MyOcelot.xml");
                c.IncludeXmlComments(xmlPath);
            });

            services.Configure<JwtTokenOptions>(Configuration.GetSection("JwtTokenOptions"));
            JwtTokenOptions tokenOptions = Configuration.GetSection("JwtTokenOptions").Get<JwtTokenOptions>();
            #region jwt验证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,//是否验证Issuer
                            ValidateAudience = true,//是否验证Audience
                            ValidateLifetime = true,//是否验证失效时间
                            ClockSkew = TimeSpan.FromSeconds(30),
                            ValidateIssuerSigningKey = true,//是否验证SecurityKey
                            ValidIssuer = tokenOptions.Issuer,
                            ValidAudience = tokenOptions.Audience,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SigningKey))
                        };
                        #region 自定义Jwt的token验证
                        options.SecurityTokenValidators.Clear();//将SecurityTokenValidators清除掉，否则它会在里面拿验证
                        options.SecurityTokenValidators.Add(new MyTokenValidator()); //自定义的MyTokenValidator验证方法
                        options.Events = new JwtBearerEvents
                        {
                            //重写OnMessageReceived
                            OnMessageReceived = context =>
                            {
                                var token = context.Request.Headers["Authorization"];
                                string jwtToken = AESCryptoHelper.Decrypt(token.FirstOrDefault());
                                context.Token = jwtToken;
                                if (context.Token == null)
                                {
                                    context.Fail("鉴权失败");
                                }
                                return Task.CompletedTask;
                            },
                            OnTokenValidated = context =>
                            {
                                try
                                {
                                    ClaimsPrincipal identity = context.Principal;
                                }
                                catch (Exception ee)
                                {
                                    context.Fail("鉴权失败:" + ee.Message);
                                }
                                return Task.CompletedTask;
                            }
                        };
                        #endregion

                    });
            #endregion
            services.AddSingleton<TokenBuilder>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseRouting();
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = "swagger";
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseStaticFiles();
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.DownstreamSwaggerHeaders = new[]
                {
                        new KeyValuePair<string, string>("Key", "Value"),
                        new KeyValuePair<string, string>("Key2", "Value2"),
                    };
            })
            .UseOcelot()
            .Wait();
        }
    }
}
