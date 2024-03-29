﻿using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Domain.Identity;
using Infrastructure.Factories;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Models;
using Sieve.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public static class ConfigureServices
    {

        //This is Extension function we use to configure Infrastructure Services instead of write it in program.cs
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                
                options.UseSqlServer(configuration.GetConnectionString("Connection1"),
                      b => b.MigrationsAssembly("Infrastructure"));
               
                
            }, ServiceLifetime.Scoped);

            services.AddScoped<ISieveProcessor, SieveProcessor>();
            services.Configure<SieveOptions>(options =>
            {
                configuration.GetSection("Sieve").Bind(options);
            });

            // services.Configure<SieveOptions>(configuration.GetSection("Sieve"));
            //services.AddScoped<ISieveProcessor, SieveProcessor>();
            // services.Configure<SieveOptions>(configuration.GetSection("Sieve"));


            services.AddScoped<IResourceTypeRepo, ResourceTypeRepository>();
            services.AddScoped<IResourceMetadataRepo, ResourceMetadataRepository>();
            services.AddScoped<IResourceRepo, ResourceRepository>();
            services.AddScoped<IResourceDataRepo, ResourceDataRepository>();
            services.AddScoped<IScheduleRepo, ScheduleRepository>();
            services.AddScoped<IScheduleItemRepo, ScheduleItemRepository>();
            services.AddScoped<AccountRepository, AccountRepository>();
            services.AddScoped(typeof(IAsyncRepository<>), typeof(CRUDRepository<>));
            services.AddScoped<IBookingItemRepo, BookItemRepository>();
            services.AddScoped<IServiceMetadataRepo, ServiceMetadaRepository>();
            services.AddScoped<IClientBookingRepo, ClientBookingRepository>();
            services.AddScoped<IApplicationUserRepo, ApplicationUserRepository>();
            services.AddScoped<IServiceRepo, ServiceRepository>();
            services.AddScoped<IAccountRepository,AccountRepository>();
            services.AddScoped<IResourceReviewRepo, ResourceReviewRepository>();
            services.AddScoped<IRegionRepository, RegionRepository>();
            services.AddScoped<IFAQRepo, FAQRepository>();
            services.AddScoped<IFAQCategoryRepo, FAQCategoryRepository>();
            services.AddScoped<IResourceSpecialCharacteristicsRepo, ResourceSpecialCharacteristicsRepository>();
            services.AddCors();
            services.AddScoped<IServiceRepo, ServiceRepository>();

            services.AddScoped<IPaymentService, StripePaymentService>();
            services.AddScoped<IPaymentService, PaypalPaymentService>();

            services.AddScoped<StripePaymentService, StripePaymentService>();
            services.AddScoped<PaypalPaymentService, PaypalPaymentService>();

            services.AddScoped<ApplicationDbContextInitializer>();
            services.AddScoped<IPayemntTransactionRepository, PaymentTransactionRepository>();
            services.AddScoped<IBookingFlowRepo, BookingFlowRepository>();


            services.AddSingleton<PaymentFactory>();

            return services;
        }

    }
}