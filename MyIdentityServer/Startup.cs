using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyIdentityServer
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

            services.AddIdentityServer()
           .AddDeveloperSigningCredential()
           //api资源
           .AddInMemoryApiResources(InMemoryConfig.GetApiResources())
           //4.0版本需要添加，不然调用时提示invalid_scope错误
           .AddInMemoryApiScopes(InMemoryConfig.GetApiScopes())
           .AddTestUsers(InMemoryConfig.Users().ToList())
           .AddInMemoryIdentityResources(InMemoryConfig.GetIdentityResources())
           .AddInMemoryClients(InMemoryConfig.GetClients());

            //获取连接串
            //string connString = _configuration.GetConnectionString("Default");
            //string migrationsAssembly = Assembly.GetEntryAssembly().GetName().Name;
            //////添加IdentityServer服务
            //services.AddIdentityServer()
            //    //添加这配置数据(客户端、资源)
            //    .AddConfigurationStore(opt =>
            //    {
            //        opt.ConfigureDbContext = c =>
            //        {
            //            c.UseSqlServer(connString, sql => sql.MigrationsAssembly(migrationsAssembly));
            //        };
            //    })
            //    //添加操作数据(codes、tokens、consents)
            //    .AddOperationalStore(opt =>
            //    {
            //        opt.ConfigureDbContext = c =>
            //        {
            //            c.UseSqlServer(connString, sql => sql.MigrationsAssembly(migrationsAssembly));
            //        };
            //        //token自动清理
            //        opt.EnableTokenCleanup = true;
            //        //token自动清理间隔：默认1H
            //        opt.TokenCleanupInterval = 3600;
            //        ////token自动清理每次数量
            //        //opt.TokenCleanupBatchSize = 100;
            //    })
            //    .AddTestUsers(InMemoryConfig.Users().ToList());

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyIdentityServer", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyIdentityServer v1"));
            }

            app.UseRouting();

            //1.启用IdentityServe4
            app.UseIdentityServer();

            //添加静态资源访问
            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
