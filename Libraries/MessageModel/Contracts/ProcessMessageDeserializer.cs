using MessageModel.Model.DataBlockModel;
using MessageModel.Model.Messages;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

namespace MessageModel.Contracts
{
    internal class ProcessMessageDeserializer : IMessageDeserializer
    {
        public MessageBase Deserialize(byte[] body)
        {
            string bodyString = Encoding.UTF8.GetString(body);
            var jsonObject = JsonConvert.DeserializeObject<JObject>(bodyString);

            byte priority = jsonObject["Priority"].ToObject<byte>();

            var processObject = jsonObject["Process"] as JObject;
            if (processObject == null)
            {
                throw new InvalidDataException("Missing 'Process' object in JSON");
            }

            short row = processObject["Row"].ToObject<short>();
            short col = processObject["Col"].ToObject<short>();
            L1L2_Process process = new L1L2_Process(row, col);

            return new ProcessMessage(process, priority);
        }
    }
}