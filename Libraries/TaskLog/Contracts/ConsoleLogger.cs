using MessageModel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskLog.Contracts
{
    public class ConsoleLogger : ILogger
    {
        private static readonly Dictionary<Severity, ConsoleColor> SeverityColors = 
            new Dictionary<Severity, ConsoleColor>
        {
            { Severity.Warning, ConsoleColor.Yellow },
            { Severity.Error, ConsoleColor.Red },
            { Severity.Info,ConsoleColor.Gray }
        };
        public void Log(LogMessage logMessage)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            ConsoleColor levelColor;

            if (SeverityColors.TryGetValue(logMessage.Level, out levelColor))
            {
                Console.Write($"{logMessage.TaskName} - ");
                Console.ForegroundColor = levelColor;
                Console.Write($"[{logMessage.Level}] ");
                Console.ForegroundColor = originalColor;
                Console.WriteLine($"{logMessage.TimeStamp}: {logMessage.Message}");
            }
            else
            {
                Console.WriteLine($"{logMessage.TaskName} - [{logMessage.Level}] {logMessage.TimeStamp}: {logMessage.Message}");
            }
        }
        public Task LogAsync(LogMessage logMessage)
        {
            Log(logMessage);
            return Task.CompletedTask;
        }
    }
}
