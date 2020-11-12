using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.Scheduling.StartParameters
{
    /// <summary>
    /// Scheduling options for a scheduler operation.
    /// </summary>
    public class SchedulerTimingOptions
    {
        /// <summary>
        /// Interval between each operation. If not set will schedule once a day.
        /// </summary>
        public SchedulerIntervalTimeSpan IntervalOption { get; private set; }
        /// <summary>
        /// Active working hours in day.
        /// </summary>
        public SchedulerWorkingTimeSpan WorkingTimeSpan { get; private set; }
        /// <summary>
        /// Creates a schedule time that will run on every interval.
        /// </summary>
        /// <param name="interval">The time between each operation.</param>
        public SchedulerTimingOptions(SchedulerIntervalTimeSpan interval)
        {
            IntervalOption = interval;
            WorkingTimeSpan = null;
        }
        /// <summary>
        /// Creates a schedule time that will run once a day in the specified active hours.
        /// </summary>
        /// <param name="workingTimeSpan">Active working hours.</param>
        public SchedulerTimingOptions(SchedulerWorkingTimeSpan workingTimeSpan)
        {
            WorkingTimeSpan = workingTimeSpan;
            IntervalOption = null;
        }
        /// <summary>
        /// Creates a schedule time that will run on every interval in the specified active hours.
        /// </summary>
        /// <param name="workingTimeSpan">Active working hours.</param>
        /// <param name="interval">The time between each operation.</param>
        public SchedulerTimingOptions(SchedulerWorkingTimeSpan workingTimeSpan, SchedulerIntervalTimeSpan interval)
        {
            WorkingTimeSpan = workingTimeSpan;
            IntervalOption = interval;
        }
    }
}
