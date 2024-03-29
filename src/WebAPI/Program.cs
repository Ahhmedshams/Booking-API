using Infrastructure;
using Application;
using System.Reflection;
using Domain.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity.EmailSettings;
using Azure.Storage.Blobs;
using Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using Application.Common.Interfaces.Services;
using Infrastructure.Services;

using Domain.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Sieve.Models;
using Sieve.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Newtonsoft.Json.Converters;
using Infrastructure.BackgroundTasks;

namespace WebAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                options.SerializerSettings.Converters.Add(new StringEnumConverter());
            });
           
            builder.Services.AddRazorPages();
            builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
            });



            




  
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            //builder.Services.AddScoped<ISieveProcessor, SieveProcessor>();
            //builder.Services.Configure<SieveOptions>(builder.Configuration.GetSection("Sieve"));



            #region AzureUpload
            builder.Services.AddSingleton(e => new BlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]));
            builder.Services.AddSingleton(e => e.GetRequiredService<BlobServiceClient>().GetBlobContainerClient(builder.Configuration["AzureStorage:ImageContainer"]));
            builder.Services.AddSingleton<UploadImage>();
            #endregion


            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(opt =>
            {
                opt.User.AllowedUserNameCharacters = null;
                opt.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Default");

            builder.Services.Configure<IdentityOptions>(opt =>
            {
                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false; //check if the request is https
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecurityKey"]))
                };
            });

            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("GuestUser", policy =>
                {
                    policy.RequireAssertion(context => true);
                });
            });



            /*            builder.Services.AddAuthentication();*/
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            builder.Services.AddHostedService<CheckProecessingBookingTask>();
            builder.Services.AddHostedService<CheckCompletedBookingsTask>();

            StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();

                using (var scope = app.Services.CreateScope())
                {
                    var initializaer = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitializer>();
                    await initializaer.InitailizeAsync();
                    await initializaer.SeedAsync();
                }

            }

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()     // Allow requests from any origin
                       .AllowAnyMethod()     // Allow all HTTP methods
                       .AllowAnyHeader();    // Allow all headers
            });
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.MapRazorPages();
            app.MapControllers();

            app.Run();
        }
    }
}