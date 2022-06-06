using Consul;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;

namespace Catalog.Api.Extensions
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
            var server = app.ApplicationServices.GetRequiredService<IServer>();
            var addressFeature = server.Features.Get<IServerAddressesFeature>();
            var address = addressFeature.Addresses;

            //Register service with consul
            var uri = new Uri(address.First());
            var registration = new AgentServiceRegistration()
            {
                ID = $"CatalogService",
                Name = "CatalogService",
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = new[] { "Catalog Service", "Catalog"}
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
