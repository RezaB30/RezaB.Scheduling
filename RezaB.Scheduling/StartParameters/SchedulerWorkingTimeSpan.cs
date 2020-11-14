using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.Scheduling.StartParameters
{
    /// <summary>
    /// Defines an active working hour time span for a scheduled operation.
    /// </summary>
    public class SchedulerWorkingTimeSpan
    {
        /// <summary>
        /// What time of the day should this operation start working.
        /// </summary>
        public virtual TimeSpan StartTime { get; private set; }
        /// <summary>
        /// What time of the day should this operation stop working.
        /// </summary>
        public virtual TimeSpan EndTime { get; private set; }
        /// <summary>
        /// Creates an active working hour time span for a scheduled operation.
        /// </summary>
        /// <param name="startTime">What time of the day should this operation start working.</param>
        /// <param name="endTime">What time of the day should this operation stop working.</param>
        public SchedulerWorkingTimeSpan(TimeSpan startTime, TimeSpan endTime)
        {
            if (startTime >= endTime || startTime.Days > 0 || endTime.Days > 0)
            {
                throw new ArgumentException("Values must be less than a full day and start time should be lesser than end time.");
            }

            StartTime = startTime;
            EndTime = endTime;
        }
    }
}
