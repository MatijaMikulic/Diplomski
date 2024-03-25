using MessageModel.Model.DataBlockModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageModel.Model.Messages
{
    public class ProcessMessage:MessageBase
    {
        public L1L2_Process Process { get; set; }
        public ProcessMessage(L1L2_Process process, byte priority) 
            : base(priority, MessageType.ProcessMessage)
        {
            Process = process;
        }
    }
}
