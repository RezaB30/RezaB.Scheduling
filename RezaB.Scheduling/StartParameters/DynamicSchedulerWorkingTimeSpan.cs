using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.Scheduling.StartParameters
{
    /// <summary>
    /// Defines a dynamic active working hour time span for a scheduled operation.
    /// </summary>
    public class DynamicSchedulerWorkingTimeSpan : SchedulerWorkingTimeSpan
    {
        /// <summary>
        /// What time of the day should this operation start working.
        /// </summary>
        public override TimeSpan StartTime
        {
            get
            {
                return _startTimeGetter();
            }
        }
        /// <summary>
        /// What time of the day should this operation stop working.
        /// </summary>
        public override TimeSpan EndTime
        {
            get
            {
                return _endTimeGetter();
            }
        }

        private Func<TimeSpan> _startTimeGetter;
        private Func<TimeSpan> _endTimeGetter;
        /// <summary>
        /// Creates a dynamic active working hour time span for a scheduled operation.
        /// </summary>
        /// <param name="startTime">Dynamic start time getter.</param>
        /// <param name="endTime">Dynamic end time getter.</param>
        public DynamicSchedulerWorkingTimeSpan(Func<TimeSpan> startTime, Func<TimeSpan> endTime) : base(startTime(), endTime())
        {
            _startTimeGetter = startTime;
            _endTimeGetter = endTime;
        }
    }
}
