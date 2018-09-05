using GraduationProjectAPI.Filters;
using GraduationProjectModels;
using GraduationProjectRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using GraduationProjectInterfaces.Controllers;
using GraduationProjectAPI.Controllers;
using GraduationProjectInterfaces.Services;
using GraduationProjectServices;
using GraduationProjectInterfaces.Repository;
using GraduationProjectInterfaces.ImageHandler;
using GraduationProjectImageHandler;

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
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        // Specifies whether the publisher will validate when validating the token.
                        ValidateIssuer = false,
                        // Will the token consumer be validated.
                        ValidateAudience = false,
                        // Will the lifetime be validated.
                        ValidateLifetime = true,
                        // Set the security key.
                        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                        // Validate the security key.
                        ValidateIssuerSigningKey = true
                    };
                });

            // Add cors attribute to all controllers.
            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("MyPolicy"));
            });

            // Enable MVC and add exception handling attribute.
            services.AddMvc(options =>
                options.Filters.Add(new ControllerExceptionFilterAttribute())).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

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
        private void RegisterDependencyInjection(IServiceCollection services)
        {
            services.AddScoped<IImageHandler, ImageHandler>();

            services.AddScoped<IRepository<User>, Repository<User>>();
            services.AddScoped<IRepository<BlankFile>, Repository<BlankFile>>();
            services.AddScoped<IRepository<Password>, Repository<Password>>();
            
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IAccountController, AccountController>();
            services.AddScoped<IUserController, UserController>();

            services.AddDbContext<GraduationProjectContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
        }
    }
}
