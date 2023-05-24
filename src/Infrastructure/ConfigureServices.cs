﻿using Infrastructure.Persistence;
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
            });
            return services;
        }
    }
}