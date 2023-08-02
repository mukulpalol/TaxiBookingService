using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TaxiBookingService.DAL;
using TaxiBookingService.DAL.Repositories;
using TaxiBookingService.DAL.RepositoriesContract;
using TaxiBookingService.Logic.Services;
using TaxiBookingService.Logic.ServicesContract;

namespace TaxiBookingService.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<TbsDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                m => m.MigrationsAssembly("TaxiBookingService.Host")
                ));

            builder.Host.ConfigureLogging(loggingProvider =>
            {
                loggingProvider.ClearProviders();
                loggingProvider.AddConsole();
                loggingProvider.AddDebug();
                loggingProvider.AddEventLog();
            });

            builder.Services.AddMvc();

            builder.Services.AddAuthorization();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                 }
                            },
                            new string[] { }
                        }
                    });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new[] { "readAccess", "writeAccess" }
                        }
                    });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                            .AddJwtBearer(options =>
                            {
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = false,
                                    ValidateAudience = false,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                                };
                            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = "TaxiBookingServiceSession";
                options.IdleTimeout = TimeSpan.FromMinutes(15);
            });

            builder.Services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                o.ReportApiVersions = true;
                o.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("ApiVersion"));

            });

            builder.Services.AddVersionedApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IDriverService, DriverService>();
            builder.Services.AddScoped<IRideService, RideService>();
            builder.Services.AddScoped<IUserService, UserService>();

            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<ICancelRepository, CancelRepository>();
            builder.Services.AddScoped<IDriverRepository, DriverRepository>();
            builder.Services.AddScoped<IRideRepository, RideRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpLogging();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.MapControllers();
            app.Run();
        }
    }
}