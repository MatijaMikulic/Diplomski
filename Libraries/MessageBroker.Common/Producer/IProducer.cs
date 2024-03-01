﻿using MessageModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MessageBroker.Common.Producer
{
    /// <summary>
    /// Defines the interface for a message producer, providing methods for sending messages to a message queue.
    /// </summary>
    public interface IProducer:IDisposable
    {
        /// <summary>
        /// Sends a message with the specified routing key.
        /// </summary>
        /// <param name="routingKey">The routing key for the message.</param>
        /// <param name="message">The message to be sent.</param>
        public void SendMessage(string routingKey, MessageBase message);

        // <summary>
        /// Opens the communication channel for the producer.
        /// </summary>
        public void OpenCommunication();

        /// <summary>
        /// Checks whether the producer is connected to the message queue.
        /// </summary>
        /// <returns>True if connected; otherwise, false.</returns>
        public bool IsConnected();

    }
}
