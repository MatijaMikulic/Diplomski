using MessageModel.Model.DataBlockModel;
using MessageModel.Model.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MessageModel.Utilities
{
    public static class MessageFactory
    {
        public static MessageBase CreateMessage(PlcData plcData)
        {
            if (plcData is L1L2_Process process)
            {
                return new ProcessMessage(process, 1);
            }
            else if (plcData is L1L2_Request request)
            {
                return new RequestMessage(request, 2);
            }
            else
            {
                throw new ArgumentException("Unsupported PlcData type");
            }
        }
    }
}
