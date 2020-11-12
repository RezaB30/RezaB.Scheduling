using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.Scheduling.StartParameters
{
    /// <summary>
    /// Defines the interval for a scheduled operation.
    /// </summary>
    public class SchedulerIntervalTimeSpan
    {
        /// <summary>
        /// The interval value.
        /// </summary>
        public TimeSpan Interval { get; private set; }
        /// <summary>
        /// Creates a scheduler interval timing option.
        /// </summary>
        /// <param name="interval">The time between each interval.</param>
        public SchedulerIntervalTimeSpan(TimeSpan interval)
        {
            Interval = interval;
        }
    }
}
