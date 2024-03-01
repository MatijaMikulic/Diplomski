using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessageModel.Model;

namespace MessageModel.Contracts
{
    /// <summary>
    /// Implementation of IMessageDeserializer for deserializing SimpleMessage objects.
    /// </summary>
    public class SimpleMessageDeserializer : IMessageDeserializer
    {
        public Model.MessageBase Deserialize(byte[] body)
        {
            string bodyString = Encoding.UTF8.GetString(body);
            var jsonObject = JsonConvert.DeserializeObject<JObject>(bodyString);

            var text = jsonObject["Text"].ToObject<string>();
            var priority = jsonObject["Priority"].ToObject<byte>();

            return new SimpleMessage(text,priority);
        }
    }
}
