﻿using Microsoft.EntityFrameworkCore;

namespace EasyTalkWeb.Persistance
{
    public static class PersistanceServiceRegistration
    {
        public static IServiceCollection AddPersistanceServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("EasyTalkConnectionString"));
            });

            return services;
        }
    }
}