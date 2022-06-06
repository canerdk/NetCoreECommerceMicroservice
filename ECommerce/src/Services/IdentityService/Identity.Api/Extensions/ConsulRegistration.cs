using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;

namespace Identity.Api.Extensions
{
    public static class ConsulRegistration
    {
        public static IServiceCollection ConfigureConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IConsulClient, ConsulClient>(c => new ConsulClient(cfg =>
            {
                var address = configuration["ConsulConfig:Address"];
                cfg.Address = new Uri(address);
            }));

            return services;
        }

        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IHostApplicationLifetime lifetime)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

    
            //Get server IP address
            var features = app.Properties["server.Features"] as FeatureCollection;
            var addresses = features.Get<IServerAddressesFeature>();
            var address = addresses.Addresses.First();

            //Register service with consul
            var uri = new Uri(address);
            var registration = new AgentServiceRegistration()
            {
                ID = $"IdentityService",
                Name = "IdentityService",
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = new[] { "Identity Service", "Identity", "JWT", "Token" }
            };

            logger.LogInformation("Registering with consul");
            consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            consulClient.Agent.ServiceRegister(registration).Wait();

            lifetime.ApplicationStopping.Register(() =>
            {
                logger.LogInformation("Deregistering from consul");
                consulClient.Agent.ServiceDeregister(registration.ID).Wait();
            });

            return app;

        }
    }
}
