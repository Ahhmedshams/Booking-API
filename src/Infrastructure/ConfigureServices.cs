﻿using Application.Common.Interfaces;
using Application.Common.Interfaces.Repositories;
using Domain.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddCors();
            services.AddScoped<ApplicationDbContextInitializer>();


            return services;
        }
    }
}