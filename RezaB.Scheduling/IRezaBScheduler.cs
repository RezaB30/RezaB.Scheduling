using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.Scheduling
{
    /// <summary>
    /// The scheduler interface.
    /// </summary>
    public interface IRezaBScheduler
    {
        /// <summary>
        /// Starts the scheduler.
        /// </summary>
        /// <param name="loggerName">The logger name. (default is the same as the name of the scheduler.)</param>
        void Start(string loggerName = null);
        /// <summary>
        /// Stops the scheduler by signaling abort to all tasks.
        /// </summary>
        void Stop();
    }
}
