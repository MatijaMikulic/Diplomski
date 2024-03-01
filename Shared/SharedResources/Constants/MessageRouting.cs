using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedResources.Constants
{
    public static class MessageRouting
    {
        public readonly static string LoggerRoutingKey = "logger-data-routing-key";
        public readonly static string LoggerQueue = "LoggerQueue";

        public readonly static string DataQueue = "DataQueue";
        public readonly static string DataRoutingKey = "data-routing-key";        
    }
}
