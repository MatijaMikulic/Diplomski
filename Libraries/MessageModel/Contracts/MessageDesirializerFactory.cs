using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageModel.Model;

namespace MessageModel.Contracts
{
    /// <summary>
    /// Factory class for creating message deserializers based on message types.
    /// </summary>
    public static class MessageDesirializerFactory
    {
        /// <summary>
        /// Gets a message deserializer based on the specified message type.
        /// </summary>
        /// <param name="messageType">The type of the message.</param>
        /// <returns>An instance of the IMessageDeserializer interface for the specified message type.</returns>
        public static IMessageDeserializer GetDeserializer(MessageType messageTyoe)
        {
            switch (messageTyoe)
            {
                case MessageType.LogMessage:
                    return new LogMessageDeserializer();
                case MessageType.SimpleMessage:
                    return new SimpleMessageDeserializer();
                case MessageType.DataBlockHeader:
                    return new DataBlockHeaderDeserializer();
                default:
                    throw new ArgumentException("Unsupported message type");
            }
        }
    }
}
