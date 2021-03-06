﻿using GraduationProjectAPI.Filters;
using GraduationProjectModels;
using GraduationProjectRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using GraduationProjectInterfaces.Controllers;
using GraduationProjectAPI.Controllers;
using GraduationProjectInterfaces.Services;
using GraduationProjectServices;
using GraduationProjectInterfaces.Repository;

namespace GraduationProjectAPI
{
    // Configuration of application.
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
            // Enable cors.
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }));

            // Add auth to app by JWT token.
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = AuthOptions.GetTokenValidationParameters();
                });

            // Add cors attribute to all controllers.
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("MyPolicy"));
            });

            // Enable MVC and add exception handling attribute.
            services.AddMvc(options =>
                options.Filters.Add(new ControllerExceptionFilterAttribute())).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            //Get and decode connection string.
            BuildAppSettingsProvider();

            // Register all dependencies.
            RegisterDependencyInjection(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseAuthentication()
                .UseCors("MyPolicy")
                .UseMvc();
        }

        // Register dependencies.
        private static void RegisterDependencyInjection(IServiceCollection services)
        {
            services.AddScoped<IRepository<User>, Repository<User>>();
            services.AddScoped<IRepository<BlankFile>, Repository<BlankFile>>();
            services.AddScoped<IRepository<Password>, Repository<Password>>();
            services.AddScoped<IRepository<BlankType>, Repository<BlankType>>();
            services.AddScoped<IRepository<QuestionEntity>, Repository<QuestionEntity>>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IAccountController, AccountController>();
            services.AddScoped<IUserController, UserController>();

            services.AddDbContext<GraduationProjectContext>(options =>
                options.UseSqlServer(AppSettings.DbConnectionString));
        }

        private void BuildAppSettingsProvider()
        {
            AppSettings.DbConnectionString =
                Defines.GetDecodedString(Configuration["AppSettings:DBConnectionString"]);

            AppSettings.MicrosoftVisionApiKey =
                Defines.GetDecodedString(Configuration["AppSettings:MicrosoftVisionApiKey"]);
        }
    }
}
