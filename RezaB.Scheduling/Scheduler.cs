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
    /// A scheduler to run tasks on.
    /// </summary>
    public class Scheduler : IRezaBScheduler
    {
        /// <summary>
        /// The time between each check to see any operation is ready to run.
        /// </summary>
        public TimeSpan CheckIntervals { get; private set; }
        /// <summary>
        /// The name of the scheduler.
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Shows if the scheduler is running or not.
        /// </summary>
        public bool IsRunning { get { return _isRunning; } protected set { _isRunning = value; } }
        /// <summary>
        /// A list of operations to run on this scheduler.
        /// </summary>
        public IEnumerable<SchedulerOperation> Operations { get; private set; }
        /// <summary>
        /// Gets the structure of operations and tasks that run on this scheduler.
        /// </summary>
        public string TaskStructure
        {
            get
            {
                return _taskStructure ?? GetOperationStructure();
            }
        }

        private volatile bool _isRunning;
        private static Logger internalLogger = null;
        private Thread mainSchedulerThread = null;
        private string _taskStructure = null;
        /// <summary>
        /// Creates a scheduler with operations to run.
        /// </summary>
        /// <param name="operations">A list of operations to run on this scheduler.</param>
        /// <param name="checkIntervals">The time between each check to see any operation is ready to run.</param>
        /// <param name="name">The name of the scheduler.</param>
        public Scheduler(IEnumerable<SchedulerOperation> operations, TimeSpan checkIntervals, string name)
        {
            if (checkIntervals == null || checkIntervals.Days > 0)
                throw new ArgumentException("Check intervals must not be null or greater than/equal to 1 day.");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");
            if (operations == null || !operations.Any())
                throw new ArgumentNullException("taskTree");

            Operations = operations;
            CheckIntervals = checkIntervals;
            Name = name;
        }
        /// <summary>
        /// Starts the scheduler.
        /// </summary>
        /// <param name="loggerName">The logger name. (default is the same as the name of the scheduler.)</param>
        public void Start(string loggerName = null)
        {
            SetLogger(loggerName);
            IsRunning = true;
            mainSchedulerThread = new Thread(new ThreadStart(schedulerMain));
            mainSchedulerThread.Name = Name;
            mainSchedulerThread.IsBackground = false;
            // start main thread
            mainSchedulerThread.Start();
        }
        /// <summary>
        /// Stops the scheduler by signaling abort to all tasks.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
            foreach (var operation in Operations)
            {
                operation.Stop();
            }
            mainSchedulerThread.Join();
        }
        /// <summary>
        /// The main thread that checks on intervals.
        /// </summary>
        private void schedulerMain()
        {
            internalLogger.Info("Scheduler started.");
            internalLogger.Info(TaskStructure);
            try
            {
                DateTime? lastLoop = null;
                while (IsRunning)
                {
                    // check interval
                    if (!lastLoop.HasValue || (DateTime.Now - lastLoop.Value) >= CheckIntervals)
                    {
                        internalLogger.Trace("Running next iteration...");

                        foreach (var operation in Operations)
                        {
                            if (!IsRunning)
                                break;

                            if (operation.CanRunNow())
                            {
                                operation.Run(internalLogger);
                            }
                        }

                        internalLogger.Trace("Iteration done...");
                        lastLoop = DateTime.Now;
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                }
            }
            catch (Exception ex)
            {
                internalLogger.Fatal(ex, "Error in the main scheduler thread.");
            }

        }
        /// <summary>
        /// Sets the internal logger.
        /// </summary>
        /// <param name="loggerName">The logger to set to.</param>
        private void SetLogger(string loggerName)
        {
            if (IsRunning)
                throw new InvalidOperationException("Cannot set logger while scheduler is running.");
            internalLogger = LogManager.GetLogger(loggerName ?? Name);
        }

        private string GetOperationStructure()
        {
            StringBuilder results = new StringBuilder();
            results.AppendLine();
            results.AppendLine(Name);
            foreach (var operation in Operations)
            {
                results.Append(operation.GetTaskStructure(1));
            }

            _taskStructure = results.ToString();
            return _taskStructure;
        }
    }
}
