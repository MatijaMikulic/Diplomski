using MessageBroker.Common.Consumer;
using MessageModel.Model.Messages;
using MessageModel.Utilities;
using Newtonsoft.Json;
using SharedResources.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskLog.Contracts;

namespace LoggerService.Services
{
    public class Service
    {

        private readonly IConsumer _consumer;
        private readonly ILogger _logger;
        public Service(IConsumer consumer,ILogger logger)
        {

            _consumer = consumer;
            _logger = logger;
        }

        public async Task Start()
        {
            await _consumer.OpenCommunication();

            ////for sync consumer
            //EventHandler<ReceivedMessageEventArgs> receiverHandler = (sender, args) =>
            //{
            //    var body = MessageDeserializationUtilities.DeserializeMessage(args.body);
            //    if (body is LogMessage logMessage)
            //    {
            //    await _logger.logAsync(logMessage)
            //    }
            //};
            //while (true)
            //{
            //    _rabbitMqListener.ReadMessageFromQueue(MessageRouting.LoggerQueue, receiverHandler);
            //}

            //async
            await _consumer.ReadMessageFromQueueAsync(MessageRouting.LoggerQueue, async (body) =>
            {
                //string message = Encoding.UTF8.GetString(body);
                var logMessage = MessageDeserializationUtilities.DeserializeMessage(body);
                if(logMessage is LogMessage l)
                {
                    await _logger.LogAsync(l);
                }
            });

        }

        public void Stop()
        {
            _consumer.Dispose();
        }
    }
}
