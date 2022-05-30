using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        RabbitMQPersistentConnection persistentConnection;
        private readonly IConnectionFactory _connectionFactory;
        private readonly IModel _consumerChannel;

        public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
            if (config.Connection != null)
            {
                var connectionJson = JsonConvert.SerializeObject(EventBusConfig.Connection, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                _connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connectionJson);
            }
            else
            {
                _connectionFactory = new ConnectionFactory();
            }

            persistentConnection = new RabbitMQPersistentConnection(_connectionFactory, config.ConnectionRetryCount);
            _consumerChannel = CreateConsumerChannel();
        }

        public override void Publish(IntegrationEvent @event)
        {
            throw new NotImplementedException();
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = ProcessEventName(typeof(T).Name);

            if (!SubsManager.HasSubscriptionsForEvent(eventName))
            {
                if (!persistentConnection.IsConnected)
                {
                    persistentConnection.TryConnect();
                }

                _consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                    durable: true, exclusive: false, autoDelete: false, arguments: null);

                _consumerChannel.QueueBind(queue: GetSubName(eventName), exchange: EventBusConfig.DefaultTopicName,
                    routingKey: eventName);
            }

            SubsManager.AddSubscription<T, TH>();
        }

        public override void UnSubscribe<T, TH>()
        {
            throw new NotImplementedException();
        }

        private IModel CreateConsumerChannel()
        {
            if (!persistentConnection.IsConnected)
            {
                persistentConnection.TryConnect();
            }

            var channel = persistentConnection.CreateModel();

            channel.ExchangeDeclare(exchange: EventBusConfig.DefaultTopicName, type: "direct");

            return channel;
        }
    }
}
