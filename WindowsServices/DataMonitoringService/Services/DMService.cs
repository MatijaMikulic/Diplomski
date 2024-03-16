using PlcCommunication;
using RabbitMQ.Client;
using S7.Net;
using MessageModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using MessageModel.Model;
using DataMonitoringService.Constants;
using SharedResources.Constants;
using MessageBroker.Common.Producer;
using PlcCommunication.Model;
using DataMonitoringService.Model;
using System.ComponentModel;

namespace DataMonitoringService.Services
{
    public class DMService
    {
        private readonly System.Timers.Timer _timer;
        private readonly IProducer _producer;
        private readonly PlcCommunicationService _plcCommunicationService;

        private readonly List <CountersState> _previousCounterStates;
        public DMService(IProducer producer, PlcCommunicationService plcCommunicationService) 
        {
            _producer = producer;
            _plcCommunicationService = plcCommunicationService;

            _timer = new System.Timers.Timer(1000) { AutoReset = true };
            _timer.Elapsed += TimerElapsed;

            //for testing - 2 DBs
            _previousCounterStates = new List <CountersState>();
            _previousCounterStates.Add(new CountersState());
            _previousCounterStates.Add(new CountersState());

        }
        public async Task Start()
        {

            _plcCommunicationService.Start();
            _plcCommunicationService.PropertyChanged += LogPlcConnectionChange;
           
            _timer.Start();
            //await _producer.OpenCommunication();
            //_producer.ConnectionShutdown += LogRabbitMqConnectionLoss;

            //if (_producer.IsConnected())
            //{
            //    _producer.SendMessage(MessageRouting.LoggerRoutingKey,
            //        new LogMessage(DataMonitoringServiceInfo.ServiceName,
            //        "Service has started.",
            //        Severity.Info, 1));
            //}
        }

        //checks counters and buffer pointer 
        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (_plcCommunicationService.IsConnectionActive())
            {
                List<DataBlockMetaData> messages = _plcCommunicationService.DataAccess.ReadDBMetaData();
                for (int i = 0; i < messages.Count; i++)
                {
                    if (IsMessageReady(messages[i], i))
                    {
                        //send to message broker
                        //_producer.SendMessage(MessageRouting.DataRoutingKey,
                        //    new DataBlockHeader(messages[i].DB, messages[i].BufferPointer,1));
                        Console.WriteLine($"READY: {messages[i].ChangeCounter},{messages[i].AuxiliaryCounter},{messages[i].BufferPointer}");
                        int messageCount = messages[i].DB - _previousCounterStates[i].PreviousBufferPointer;
                    }
                    else
                    {
                        Console.WriteLine($"NOT READY: {messages[i].ChangeCounter},{messages[i].AuxiliaryCounter},{messages[i].BufferPointer}");
                    }
                    UpdatePreviousCounters(messages[i], i);
                }

            }
        }
        public void Stop()
        {
            if(_producer.IsConnected()) 
            {
                _producer.SendMessage(MessageRouting.LoggerRoutingKey,
               new LogMessage(DataMonitoringServiceInfo.ServiceName,
               "Service has stopped.",
               Severity.Info, 1));
            } 
            _plcCommunicationService.Dispose();
            _timer.Stop();
            _timer.Dispose();
            _producer.Dispose();
   
        }

        private void LogPlcConnectionChange(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(_plcCommunicationService.IsConnected))
            {
                bool isConnected = _plcCommunicationService.IsConnected;
                if (isConnected)
                {
                    Console.WriteLine("Established connection to plc!");
                }
                else
                {
                    Console.WriteLine("Lost connection to plc!");
                }
            }
            
        }
        private void LogRabbitMqConnectionLoss(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Lost connection to rabbit mq server!");
        }

        public bool HaveCountersChanged(short ChangeCounter, short AuxiliaryCounter, int i)
        {
            return ChangeCounter != _previousCounterStates[i].PreviousChangeCounter
              || AuxiliaryCounter != _previousCounterStates[i].PreviousAuxiliaryCounter;
        }
        public void UpdatePreviousCounters(DataBlockMetaData message, int i)
        {
            _previousCounterStates[i].PreviousChangeCounter = message.ChangeCounter;
            _previousCounterStates[i].PreviousAuxiliaryCounter = message.AuxiliaryCounter;
            _previousCounterStates[i].PreviousBufferPointer = message.BufferPointer;
        }
        public bool IsMessageReady(DataBlockMetaData message, int i)
        {
            return HaveCountersChanged(message.ChangeCounter,message.AuxiliaryCounter,i)
                && message.ChangeCounter == message.AuxiliaryCounter;
        }
    }
}
