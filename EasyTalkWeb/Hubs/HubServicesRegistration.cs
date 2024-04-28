namespace EasyTalkWeb.Hubs
{
    public static class HubServicesRegistration
    {
        public static IServiceCollection AddHubServices(this IServiceCollection services)
        {
            services.AddTransient<ChatHub>();
            return services;
        }
    }
}
