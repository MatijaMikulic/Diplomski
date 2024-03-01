namespace MessageModel.Model
{

    public enum MessageType
    {
        LogMessage,
        SimpleMessage,
        DataBlockHeader
    }
    public class MessageBase
    {
        public byte Priority { get; set; }
        public MessageType MessageType { get; set; }

        public MessageBase(byte priority, MessageType messageType)
        {
            Priority = priority;
            MessageType = messageType;
        }
    }
}