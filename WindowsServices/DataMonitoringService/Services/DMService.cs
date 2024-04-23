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
using DataMonitoringService.Constants;
using SharedResources.Constants;
using MessageBroker.Common.Producer;
using PlcCommunication.Model;
using DataMonitoringService.Model;
using System.ComponentModel;
using MessageModel.Model.Messages;
using MessageModel.Model.DataBlockModel;
using MessageModel.Utilities;
using RabbitMQ.Client.Exceptions;
using PlcCommunication.Constants;

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

            if(_plcCommunicationService.IsConnected)
            {
                try
                {
                    List<DataBlockMetaData> messages = _plcCommunicationService.DataAccess.ReadDBMetaData();
                }
                catch (PlcException ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            _timer.Start();
            await _producer.OpenCommunication();
            _producer.ConnectionShutdown += LogRabbitMqConnectionLoss;

            if (_producer.IsConnected())
            {
                _producer.SendMessage(MessageRouting.LoggerRoutingKey,
                    new LogMessage(DataMonitoringServiceInfo.ServiceName,
                    "Service has started.",
                    Severity.Info, 1));
            }
        }

        //checks counters and buffer pointer 
        private void TimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if(!_plcCommunicationService.IsConnected)
            {
                return;
            }

            List<DataBlockMetaData> counterStates = new List<DataBlockMetaData>();
            //Attempt to read counters
            try
            {
                counterStates = _plcCommunicationService.DataAccess.ReadDBMetaData();
            }
            catch (PlcException ex)
            {
                //Console.WriteLine(ex.Message);
                return;
            }

            //check all dbs for potential new data
            for (int i = 0; i < counterStates.Count; i++)
            {
                //if change counter or aux counter changes
                if (HaveCountersChanged(counterStates[i], i))
                {
                    //number of new messages (e.g. if the previous counter value was 65535 and the current is 0, the total number of new messages to be processed is 1.
                    int messagesReady = (counterStates[i].AuxiliaryCounter - _previousCounterStates[i].PreviousAuxiliaryCounter + ushort.MaxValue +1 ) % (ushort.MaxValue +1);
                    //if there are more new messages than the buffer size than set the number of new messages to be equal to BufferSize
                    //(because the newest messages have overwritten the old ones)
                    if(messagesReady > DataBlockInfo.BufferSize)
                    {
                        messagesReady = DataBlockInfo.BufferSize;
                    }
                    //if there are any new messages
                    if(messagesReady >= 1)
                    {
                        //find the element of the first added new message
                        int startPointer = (counterStates[i].BufferPointer - messagesReady + DataBlockInfo.BufferSize) % DataBlockInfo.BufferSize;
                        for (int j = 0; j < messagesReady; j++)
                        {
                            //ensure the pointer is within the size of buffer (e.g. goes from 1 to 5)
                            int pointer = (startPointer + j + DataBlockInfo.BufferSize) % DataBlockInfo.BufferSize + 1;

                            //create a new message 
                            DataBlockHeader dbheader = new DataBlockHeader(counterStates[i].DB, (ushort)pointer, 1);
                            try
                            {
                                _producer.SendMessage(MessageRouting.DataRoutingKey, dbheader);
                            }
                            catch(RabbitMQClientException ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            Console.WriteLine($"{counterStates[i].AuxiliaryCounter},{pointer}");
                        }
                        Console.WriteLine("-------------------");
                    }

                }
                //keep track of the previous counter values
                UpdatePreviousCounters(counterStates[i], i);
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
                    Console.WriteLine($"Established connection to plc!");
                }
                else
                {
                    Console.WriteLine($"Lost connection to plc!");
                }
            }
            
        }
        private void LogRabbitMqConnectionLoss(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("Lost connection to rabbit mq server!");
        }
        private bool HaveCountersChanged(DataBlockMetaData counterState, int i)
        {
            return counterState.ChangeCounter != _previousCounterStates[i].PreviousChangeCounter
              || counterState.AuxiliaryCounter != _previousCounterStates[i].PreviousAuxiliaryCounter;
        }
        private void UpdatePreviousCounters(DataBlockMetaData counterState, int i)
        {
            _previousCounterStates[i].PreviousChangeCounter = counterState.ChangeCounter;
            _previousCounterStates[i].PreviousAuxiliaryCounter = counterState.AuxiliaryCounter;
            _previousCounterStates[i].PreviousBufferPointer = counterState.BufferPointer;
        }
        private bool IsMessageReady(DataBlockMetaData counterState, int i)
        {
            return HaveCountersChanged(counterState, i)
                && counterState.ChangeCounter == counterState.AuxiliaryCounter;
        }
    }
}
