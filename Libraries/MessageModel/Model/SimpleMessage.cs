using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageModel.Model
{
    public class SimpleMessage : MessageBase
    {
        public string Text { get; set; }

        public SimpleMessage(string text, byte priority)
            : base(priority, MessageType.SimpleMessage)
        {
            Text = text;
        }
    }
}
