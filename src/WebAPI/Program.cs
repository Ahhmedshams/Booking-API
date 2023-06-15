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

namespace WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddRazorPages();
            builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddScoped<IMailService, MailService>();
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

            #region AzureUpload
            builder.Services.AddSingleton(e => new BlobServiceClient(builder.Configuration["AzureStorage:ConnectionString"]));
            builder.Services.AddSingleton(e => e.GetRequiredService<BlobServiceClient>().GetBlobContainerClient(builder.Configuration["AzureStorage:ImageContainer"]));
            builder.Services.AddSingleton<UploadImage>(); 
            #endregion

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>("Default");
            builder.Services.Configure<IdentityOptions>(opt =>
            {
                /*opt.Password.RequireDigit = false;
                 opt.Password.RequireLowercase = false;
                 opt.Password.RequireUppercase = false;
                 opt.Password.RequireNonAlphanumeric = false;
                 opt.Password.RequiredLength = 8;*/
                opt.User.RequireUniqueEmail = true;
                opt.SignIn.RequireConfirmedEmail = true;
                //opt.Password.RequiredUniqueChars = 1;
            });
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                //options.RequireHttpsMetadata = true; check if the request is https
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecurityKey"]))
                };
            });



            /*            builder.Services.AddAuthentication();*/
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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