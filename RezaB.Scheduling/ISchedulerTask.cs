using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.Scheduling
{
    /// <summary>
    /// An interface for all scheduler tasks.
    /// </summary>
    public interface ISchedulerTask
    {
        /// <summary>
        /// Signals to run the scheduled task.
        /// </summary>
        /// <param name="logger">NLog logger name for logging.</param>
        void Run(Logger logger);
        /// <summary>
        /// Signals to stop the current task flow.
        /// </summary>
        void Stop();
        /// <summary>
        /// Wait for the task to complete.
        /// </summary>
        void Wait();
    }
}
