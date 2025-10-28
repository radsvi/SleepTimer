using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class LogEntry
    {
        [JsonConstructor]
        protected LogEntry(LogTypes type, string? text)
        {
            Date = DateTime.Now;
            Text = text;
            Type = type;
        }

        [JsonProperty] public DateTime Date { get; private set; }
        [JsonProperty] public string? Text { get; private set; }
        [JsonProperty] public LogTypes Type { get; set; }
    }
    public class LogEntryStarted() : LogEntry(LogTypes.Started, "Timer started") {}
    public class LogEntryExtended() : LogEntry(LogTypes.Extended, "Timer extended") {}
    public class LogEntryEnteringStandby() : LogEntry(LogTypes.Standby, "Entering standby") {}
    public class LogEntryTimerFinished() : LogEntry(LogTypes.Finished, "Timer finished") {}
}
