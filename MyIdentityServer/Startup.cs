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
           //api��Դ
           .AddInMemoryApiResources(InMemoryConfig.GetApiResources())
           //4.0�汾��Ҫ��ӣ���Ȼ����ʱ��ʾinvalid_scope����
           .AddInMemoryApiScopes(InMemoryConfig.GetApiScopes())
           .AddTestUsers(InMemoryConfig.Users().ToList())
           .AddInMemoryIdentityResources(InMemoryConfig.GetIdentityResources())
           .AddInMemoryClients(InMemoryConfig.GetClients());

            //��ȡ���Ӵ�
            //string connString = _configuration.GetConnectionString("Default");
            //string migrationsAssembly = Assembly.GetEntryAssembly().GetName().Name;
            //////���IdentityServer����
            //services.AddIdentityServer()
            //    //�������������(�ͻ��ˡ���Դ)
            //    .AddConfigurationStore(opt =>
            //    {
            //        opt.ConfigureDbContext = c =>
            //        {
            //            c.UseSqlServer(connString, sql => sql.MigrationsAssembly(migrationsAssembly));
            //        };
            //    })
            //    //��Ӳ�������(codes��tokens��consents)
            //    .AddOperationalStore(opt =>
            //    {
            //        opt.ConfigureDbContext = c =>
            //        {
            //            c.UseSqlServer(connString, sql => sql.MigrationsAssembly(migrationsAssembly));
            //        };
            //        //token�Զ�����
            //        opt.EnableTokenCleanup = true;
            //        //token�Զ���������Ĭ��1H
            //        opt.TokenCleanupInterval = 3600;
            //        ////token�Զ�����ÿ������
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

            //1.����IdentityServe4
            app.UseIdentityServer();

            //��Ӿ�̬��Դ����
            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
