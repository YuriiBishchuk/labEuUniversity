using Lab.API.Helpers;
using Lab.Business;
using Lab.DataAccess;
using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Text;

namespace Lab.API
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("MyPolicy", builder => builder.WithOrigins("http://localhost:4200", "https://localhost:4200")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.RegisterBusinessModule();
            //services.RegisterDataAccessModule(Configuration);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
           
                var secretKey = Environment.GetEnvironmentVariable("AZURE_JWT_SECRETKEY")
                   ?? Configuration["Jwt:SecretKey"];
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

            // Add support for serving SPA
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist/client-app/browser";
            });

            MappingConfig.RegisterMappings();
            services.AddMapster();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppDbContext dbContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseCors("MyPolicy");

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Middleware для перенаправлення всіх запитів, які не починаються з "/api", до SPA
            app.Use(async (context, next) =>
            {
                if (!context.Request.Path.StartsWithSegments("/api"))
                {
                    var filePath = Path.Combine(env.ContentRootPath, "ClientApp", "dist", "client-app", "browser", context.Request.Path.Value.TrimStart('/'));

                    if (File.Exists(filePath))
                    {
                        // Встановлення коректного MIME-типу
                        var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
                        var contentType = fileExtension switch
                        {
                            ".js" => "application/javascript",
                            ".css" => "text/css",
                            ".html" => "text/html",
                            ".png" => "image/png",
                            ".jpg" => "image/jpeg",
                            ".jpeg" => "image/jpeg",
                            ".gif" => "image/gif",
                            ".svg" => "image/svg+xml",
                            _ => "application/octet-stream",
                        };

                        context.Response.ContentType = contentType;
                        await context.Response.SendFileAsync(filePath);
                        return;
                    }
                    else
                    {
                        context.Request.Path = "/index.html";
                    }
                }

                await next();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment() && IsDebugging())
                {
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
                }
            });
         //   dbContext.Database.Migrate();
        }

        private bool IsDebugging()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            return environment?.ToLowerInvariant() == "debug";
        }
    }

}
