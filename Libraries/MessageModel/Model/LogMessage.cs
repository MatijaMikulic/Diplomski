using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageModel.Model
{
    public enum Severity
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }
    public class LogMessage : MessageBase
    {
        public string TaskName { get; set; }
        public string Message { get; set; }
        public DateTime TimeStamp { get; set; }
        public Severity Level { get; set; }

        public LogMessage(string taskName, string message,  Severity level, byte priority, DateTime? timestamp = null)
            : base(priority, MessageType.LogMessage)
        {
            TaskName = taskName;
            Message = message;
            TimeStamp = timestamp ?? DateTime.Now; ;
            Level = level;
        }

        public override string ToString()
        {
            return $"{TimeStamp} [{Level}]: {Message}\n";
        }
    }

}
