using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Scheduling.StartParameters;

namespace RezaB.Scheduling
{
    public class SchedulerOperation : SchedulerTask
    {
        public SchedulerWorkingTimeSpan ActiveTimeSpan { get; private set; }

        public SchedulingType SchedulingType { get; private set; }

        public SchedulerOperation(string name, AbortableTask operation, SchedulerWorkingTimeSpan activeTimeSpan, SchedulingType schedulingType, ushort retryCount = 0, IEnumerable<SchedulerTask> followingTasks = null) : base(name, operation, retryCount, followingTasks)
        {
            ActiveTimeSpan = activeTimeSpan;
            SchedulingType = schedulingType;
        }

        public bool CanRunNow()
        {
            if (IsRunning || IsStopped)
                return false;

            return (ActiveTimeSpan.StartTime <= DateTime.Now.TimeOfDay && ActiveTimeSpan.EndTime > DateTime.Now.TimeOfDay) // active work time
                && (SchedulingType == SchedulingType.EveryIteration || (SchedulingType == SchedulingType.OnceADay && (!LastOperationTime.HasValue || LastOperationTime.Value.Date != DateTime.Today))); // general work type
        }
    }
}
