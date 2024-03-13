using MessageBroker.Common.Consumer;
using MessageBroker.Common.Producer;
using MessageManagerService.Constants;
using MessageModel.Model;
using MessageModel.Utilities;
using PlcCommunication;
using SharedResources.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageManagerService.Services
{
    public class MMService
    {
        private readonly IProducer _producer;
        private readonly IConsumer _consumer;
        private readonly PlcCommunicationService _plcCommunicationService;
        public MMService(IProducer producer, IConsumer consumer, PlcCommunicationService plcCommunicationService)
        {
            _producer = producer;
            _consumer = consumer;
            _plcCommunicationService = plcCommunicationService;
        }

        public async Task Start()
        {
            await _producer.OpenCommunication();
            await _consumer.OpenCommunication();  
            //_producer.SendMessage(MessageRouting.LoggerRoutingKey,
            //    new LogMessage(MessageManagerInfo.ServiceName, 
            //    "Service has started.", 
            //    Severity.Info, 1));

            await _consumer.ReadMessageFromQueueAsync(MessageRouting.DataQueue, async (body) =>
            {
                var message = MessageDeserializationUtilities.DeserializeMessage(body);
                Console.WriteLine(message.ToString());

                //await Task.CompletedTask;
            });
        }


        public void Stop()
        {
            _producer.SendMessage(MessageRouting.LoggerRoutingKey,
                new LogMessage(MessageManagerInfo.ServiceName, 
                "Service has stopped.", 
                Severity.Info, 1));

            _plcCommunicationService.Dispose();
            _producer.Dispose();
            _consumer.Dispose();

        }

    }
}
