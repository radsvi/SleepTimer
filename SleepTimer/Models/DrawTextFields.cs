using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public class DrawTextFields
    {
        public DrawTextFields(double value, string subText)
        {
            TextValue = value.ToString("N0");
            SubText = subText;
        }
        public DrawTextFields(TimeSpan value, string subText)
        {
            TextValue = $"{(int)value.TotalMinutes}:{value.Seconds.ToString("D2")}";
            SubText = subText;
        }

        public string TextValue { get; set; }
        public string SubText { get; set; }
    }
}
