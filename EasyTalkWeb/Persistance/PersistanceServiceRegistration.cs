using EasyTalkWeb.Identity.EmailHost;
using EasyTalkWeb.Models.Repositories;
using Microsoft.EntityFrameworkCore;

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
            services.AddTransient<JobPostRepository>();
            services.AddTransient<TechRepository>();
            services.AddTransient<PersonRepository>();
            services.AddTransient<ChatRepository>();
            services.AddTransient<MailService>();
            services.AddTransient<ProposalRepository>();
            services.AddTransient<MessageRepository>();

            return services;
        }
    }
}
