using EasyTalkWeb.Identity.EmailHost;
using EasyTalkWeb.Models.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddTransient<AppDbContext>();
            services.AddTransient<FreelancerRepository>();
            services.AddTransient<ClientRepository>();
            services.AddTransient<ProposalRepository>();
            services.AddTransient<JobPostRepository>();
            services.AddTransient<ITechRepository, TechRepository>();
            services.AddTransient<MailService>();


            return services;
        }
    }
}
