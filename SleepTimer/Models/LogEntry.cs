using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class LogEntry
    {
        public LogEntry(DateTime date, string? text)
        {
            Date = date;
            Text = text;
        }

        public DateTime Date { get; private set; }
        public string? Text { get; private set; }
    }
}
