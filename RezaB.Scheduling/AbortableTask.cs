using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.Scheduling
{
    /// <summary>
    /// The base class for defining scheduler tasks.
    /// </summary>
    public abstract class AbortableTask
    {
        /// <summary>
        /// Gets a signal to whether this task is aborted by the scheduler. Use this value to abort your task in logical places and avoid data loss.
        /// </summary>
        protected volatile bool _isAborted = false;
        /// <summary>
        /// Signals to abort this task. Used by the scheduler
        /// </summary>
        public void Abort() { _isAborted = true; }
        /// <summary>
        /// The main body of your task. Use the '_isAborted' flag to check if you should abort this task.
        /// </summary>
        /// <returns>If this operation completed successfully.</returns>
        public abstract bool Run();
    }
}
