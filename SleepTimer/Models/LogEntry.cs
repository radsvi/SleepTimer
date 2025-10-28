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
        protected LogEntry(LogEntries entry, string? text)
        {
            Date = DateTime.Now;
            Text = text;
        }

        [JsonProperty] public DateTime Date { get; private set; }
        [JsonProperty] public string? Text { get; private set; }
        [JsonProperty] public LogEntries Entry { get; private set; }
    }
    public class LogEntryStarted() : LogEntry(LogEntries.Started, "Timer started") {}
    public class LogEntryExtended() : LogEntry(LogEntries.Extended, "Timer extended") {}
    public class LogEntryEnteringStandby() : LogEntry(LogEntries.EnteringStandby, "Entering standby") {}
    public class LogEntryTimerFinished() : LogEntry(LogEntries.TimerFinished, "Timer finished") {}
}
