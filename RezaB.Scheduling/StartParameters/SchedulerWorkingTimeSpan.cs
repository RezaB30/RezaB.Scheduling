using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.Scheduling.StartParameters
{
    public class SchedulerWorkingTimeSpan
    {
        public TimeSpan StartTime { get; private set; }

        public TimeSpan EndTime { get; private set; }

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
