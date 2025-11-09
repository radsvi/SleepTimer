using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SleepTimer.Models
{
    public static class ResultPassingHelper
    {
        public static TaskCompletionSource<double>? CurrentTCS { get; set; }
    }
}
