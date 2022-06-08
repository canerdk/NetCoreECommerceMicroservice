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

        public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app, IConfiguration configuration)
        {
            var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

            var loggingFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            var logger = loggingFactory.CreateLogger<IApplicationBuilder>();

            var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
            //Get server IP address
            //var features = app.Properties["server.Features"] as FeatureCollection;
            //var addresses = features.Get<IServerAddressesFeature>();
            //var address = addresses.Addresses.First();

            //Register service with consul
            var uri = new Uri(configuration["ConsulConfig:ServiceUrl"]);
            var registration = new AgentServiceRegistration()
            {
                ID = configuration["ConsulConfig:ServiceId"],
                Name = configuration["ConsulConfig:ServiceName"],
                Address = $"{uri.Host}",
                Port = uri.Port,
                Tags = new[] { "Catalog Service", "Catalog" }
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
