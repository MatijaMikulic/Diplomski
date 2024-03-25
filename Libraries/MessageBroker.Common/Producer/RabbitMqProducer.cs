using MessageBroker.Common.Configurations;
using MessageModel.Model.Messages;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageBroker.Common.Producer
{
    /// <summary>
    /// Implementation of the IProducer interface for sending messages to RabbitMQ.
    /// </summary>
    public class RabbitMqProducer:IProducer
    {
        private IModel? _model;
        private IConnection? _connection;
        private readonly Dictionary<string, string> _routingKeyQueueMap;
        private readonly string _exchangeName;
        private IRabbitMqService _rabbitMqService;
        public event EventHandler<ShutdownEventArgs>? ConnectionShutdown;

        /// <summary>
        /// Initializes a new instance of the RabbitMqProducer class.
        /// </summary>
        /// <param name="rabbitMqService">The RabbitMQ service used for creating connections.</param>
        /// <param name="options">The RabbitMQ model settings.</param>
        public RabbitMqProducer(IRabbitMqService rabbitMqService, IOptions<RabbitMqModelSettings> options)
        {
            _rabbitMqService = rabbitMqService;
            _model = null;
            _connection = null;
            var _modelSettings = options.Value;
            _routingKeyQueueMap = _modelSettings.RoutingKeyQueueMap;
            _exchangeName = _modelSettings.ExchangeName;
        }
        /// <summary>
        /// Opens the communication channel by creating connections, declaring exchanges, and setting up queues.
        /// </summary>
        public async Task OpenCommunication()
        {
            const int delaySeconds = 2;
            while (true)
            {
                try
                {
                    _connection = _rabbitMqService.CreateConnection();
                    _model = _connection.CreateModel();
                    _connection.ConnectionShutdown += connection_ConnectionShutdown;

                    _model.ExchangeDeclare(_exchangeName, ExchangeType.Direct);

                    foreach (var (routingKey, queueName) in _routingKeyQueueMap)
                    {
                        SetupQueue(queueName, routingKey);
                    }
                    //return Task.CompletedTask;
                    break;
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.ToString());
                    await Task.Delay(delaySeconds * 1000);
                }
            }
        }
        private void connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            string reson = e.ToString();
            ConnectionShutdown?.Invoke(sender,e);
        }

        public bool IsConnected()
        {
            if(_connection != null)
            {
                return _connection.IsOpen;
            }
            else
            {
                return false;
            }
        }
        private void SetupQueue(string queueName, string routingKey)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            args["x-max-priority"] = 10;
            _model.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false, args);
            _model.QueueBind(queueName, _exchangeName, routingKey);
        }

        /// <summary>
        /// Sends a message with the specified routing key.
        /// </summary>
        /// <param name="routingKey">The routing key for the message.</param>
        /// <param name="message">The message to be sent.</param>
        public void SendMessage(string routingKey, MessageBase message)
        {
            var properties = _model.CreateBasicProperties();
            properties.Persistent = true;
            properties.Priority = message.Priority;

            if (_routingKeyQueueMap.TryGetValue(routingKey, out var queueName))
            {
                byte[] body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
                _model.BasicPublish(_exchangeName, routingKey, properties, body);
            }
            else
            {
                //in case a routing key doesn't exist
            }
        }
        public void Dispose()
        {
            if (_model != null && _model.IsOpen)
            {
                _model.Close();
            }
            if (_connection != null && _connection.IsOpen)
            {
                _connection.Close();
            }
        }
        
    }
}
