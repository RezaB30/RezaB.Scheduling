using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Scheduling.StartParameters;

namespace RezaB.Scheduling
{
    /// <summary>
    /// A scheduled operation that will be run by a scheduler.
    /// </summary>
    public class SchedulerOperation : SchedulerTask
    {
        /// <summary>
        /// Scheduling time.
        /// </summary>
        public SchedulerTimingOptions TimingOptions { get; set; }
        /// <summary>
        /// Creates a schedule operation.
        /// </summary>
        /// <param name="name">Name of the operation.</param>
        /// <param name="operation">The task to run.</param>
        /// <param name="timingOptions">Schedule timing options.</param>
        /// <param name="retryCount">How many retries should this operation do before passing.</param>
        /// <param name="followingTasks">A list of tasks to be run after this operation is either completed or ran out of retries.</param>
        /// <param name="runFollowingOnlyOnSuccess">If following task should only run if this operation completes successfully.</param>
        public SchedulerOperation(string name, AbortableTask operation, SchedulerTimingOptions timingOptions, ushort retryCount = 0, IEnumerable<SchedulerTask> followingTasks = null, bool runFollowingOnlyOnSuccess = false) : base(name, operation, retryCount, followingTasks, runFollowingOnlyOnSuccess)
        {
            TimingOptions = timingOptions;
        }
        /// <summary>
        /// Determines whether this operation can run now.
        /// </summary>
        /// <returns></returns>
        public bool CanRunNow()
        {
            if (IsRunning || IsStopped)
                return false;

            var workingPeriodIsValid = true;
            if (TimingOptions.WorkingTimeSpan != null)
            {
                workingPeriodIsValid = (TimingOptions.WorkingTimeSpan.StartTime <= DateTime.Now.TimeOfDay && TimingOptions.WorkingTimeSpan.EndTime > DateTime.Now.TimeOfDay); // active work time
            }
            if (TimingOptions.IntervalOption != null)
            {
                return workingPeriodIsValid && (!LastOperationTime.HasValue || LastOperationTime.Value.Add(TimingOptions.IntervalOption.Interval) <= DateTime.Now); // valid interval
            }
            else
            {
                return workingPeriodIsValid && (!LastOperationTime.HasValue || LastOperationTime.Value.Date != DateTime.Today); // once a day
            }
        }
    }
}
