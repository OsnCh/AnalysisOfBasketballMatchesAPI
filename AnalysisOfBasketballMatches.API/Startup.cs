using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Swagger;

namespace AnalysisOfBasketballMatches.API
{
    public class Startup
    {
        private readonly IHostingEnvironment _env;
        private readonly IServiceScopeFactory _serviceFactory;

        public Startup(IHostingEnvironment env, IServiceScopeFactory serviceScopeFactory)
        {
            _env = env;
            _serviceFactory = serviceScopeFactory;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = Configuration.GetSection("AppSettings");
            var adminDetailOprions = Configuration.GetSection("AdminDetailOptions");

            services.Configure<AdminDetailOptions>(adminDetailOprions);

            Infrastructure.Startup.Configure(appSettings["DbConnectionString"], services);
            ApplicationCore.Startup.Configure(services);

            services.AddCors(config =>
                config.AddPolicy("policy",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()));

            services.AddAuthorization(auth =>
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme‌​)
                    .RequireAuthenticatedUser().Build()));

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ValidIssuer = appSettings["JwtIssuer"],
                        ValidAudience = appSettings["JwtIssuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings["JwtKey"])),
                        ClockSkew = TimeSpan.Zero
                    };
                });

            services.AddMvc(options => { })
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                });

            services.AddMvcCore().AddJsonFormatters();

            services.AddSwaggerGen(
                options =>
                {
                    options.SwaggerDoc("AnalysisOfBasketballMatches", new Info()
                    {
                        Title = "Analysis Of Basketball Matches API",
                        Version = $"v1.0",
                        Description = "API Documentation - Analysis Of Basketball Matches",
                    });

                    var security = new Dictionary<string, IEnumerable<string>>
                    {
                        {"Bearer", new string[] { }},
                    };

                    options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                    {
                        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = "header",
                        Type = "apiKey"
                    });
                    options.AddSecurityRequirement(security);
                });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            var appSettings = Configuration.GetSection("AppSettings");
            Infrastructure.Startup.EnsureUpdate(appSettings["DbConnectionString"], serviceProvider);

            app.UseCors("policy");
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.RoutePrefix = string.Empty;
                    options.SwaggerEndpoint($"/swagger/AnalysisOfBasketballMatches/swagger.json", "Analysis Of Basketball Matches");
                });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Default",
                    template: "{controller=Home}/{action=Index}/{id?}"
                );
            });
            app.UseAuthentication();

            ApplicationCore.Startup.CreateRoles(serviceProvider, Configuration).Wait();
        }
    }
}
