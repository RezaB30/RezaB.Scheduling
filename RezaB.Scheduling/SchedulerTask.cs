using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Threading;

namespace RezaB.Scheduling
{
    /// <summary>
    /// A scheduler task that can have following tasks.
    /// </summary>
    public class SchedulerTask : ISchedulerTask
    {
        /// <summary>
        /// Name of the task.
        /// </summary>
        public string Name { get; protected set; }
        /// <summary>
        /// Max number of retries.
        /// </summary>
        public ushort RetryCount { get; protected set; }
        /// <summary>
        /// The list of following tasks.
        /// </summary>
        public IEnumerable<SchedulerTask> FollowingTasks { get; protected set; }
        /// <summary>
        /// If this task is stopped.
        /// </summary>
        public bool IsStopped { get { return _isStopped; }  protected set { _isStopped = value; } }
        /// <summary>
        /// If this task is currently running.
        /// </summary>
        public bool IsRunning { get { return _isRunning; } protected set { _isRunning = value; } }
        /// <summary>
        /// If following task should only run if this operation completes successfully.
        /// </summary>
        public bool RunFollowingOnlyOnSuccess { get; protected set; }
        /// <summary>
        /// Last time the operation completed. (successful or not)
        /// </summary>
        public DateTime? LastOperationTime { get; protected set; }

        protected volatile bool _isStopped;
        protected volatile bool _isRunning;
        protected Thread _internalThread = null;
        protected AbortableTask _operation = null;
        /// <summary>
        /// Creates a scheduled task to be scheduled after an operation is done.
        /// </summary>
        /// <param name="name">Name of the task.</param>
        /// <param name="operation">The task to run.</param>
        /// <param name="retryCount">How many retries should this operation do before passing.</param>
        /// <param name="followingTasks">A list of tasks to be run after this operation is either completed or ran out of retries.</param>
        /// <param name="runFollowingOnlyOnSuccess">If following task should only run if this operation completes successfully.</param>
        public SchedulerTask(string name, AbortableTask operation, ushort retryCount = 0, IEnumerable<SchedulerTask> followingTasks = null, bool runFollowingOnlyOnSuccess = false)
        {
            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            Name = name;
            RetryCount = retryCount;
            FollowingTasks = followingTasks;
            _operation = operation;
            RunFollowingOnlyOnSuccess = runFollowingOnlyOnSuccess;
        }
        /// <summary>
        /// Run the scheduled task.
        /// </summary>
        /// <param name="logger">The name of the logger. (NLog)</param>
        public void Run(Logger logger)
        {
            IsStopped = false;
            IsRunning = true;
            LastOperationTime = null;
            _internalThread = new Thread(new ThreadStart(() => internalOperation(logger)));
            _internalThread.IsBackground = true;
            _internalThread.Name = Name;
            _internalThread.Start();
        }

        protected void internalOperation(Logger logger)
        {
            logger.Info("Started task...");
            var taskWasSuccessful = false;
            // run main task with retries
            for (int i = 0; i <= RetryCount; i++)
            {
                try
                {
                    // if main task is done properly
                    if (IsStopped || _operation.Run())
                    {
                        taskWasSuccessful = true;
                        break;
                    }
                    // main task was not done properly
                    logger.Warn($"Task has not been done properly, will retry {RetryCount - i} more times.");
                }
                catch (Exception ex)
                {
                    // exception on main task
                    logger.Error(ex, $"Task threw an exception, will retry {RetryCount - i} more times.");
                }
            }
            // check for only follow on success option
            if (!RunFollowingOnlyOnSuccess || taskWasSuccessful)
            {
                // run following tasks
                if (FollowingTasks != null)
                {
                    foreach (var task in FollowingTasks)
                    {
                        if (!IsStopped)
                        {
                            task.Run(logger);
                        }
                    }
                    // wait for them all to complete
                    foreach (var task in FollowingTasks)
                    {
                        task.Wait();
                    }
                }
            }
            LastOperationTime = DateTime.Now;
            IsRunning = false;
            logger.Info("Task done.");
        }
        /// <summary>
        /// Stops the task if it is running.
        /// </summary>
        public void Stop()
        {
            IsStopped = true;
            _operation.Abort();
            if (FollowingTasks != null)
            {
                foreach (var task in FollowingTasks)
                {
                    task.Stop();
                }
            }
            Wait();
        }
        /// <summary>
        /// Waits for all threads to return.
        /// </summary>
        public void Wait()
        {
            if (_internalThread != null)
                _internalThread.Join();
        }

        internal string GetTaskStructure(int level = 0)
        {
            StringBuilder results = new StringBuilder();
                results.Append("|--");
            for (int i = 0; i < level-1; i++)
            {
                results.Append("---");
            }
            results.AppendLine($"{Name}");
            if(FollowingTasks != null)
            {
                foreach (var task in FollowingTasks)
                {
                    results.Append(task.GetTaskStructure(level + 1));
                }
            }
            return results.ToString();
        }
    }
}
