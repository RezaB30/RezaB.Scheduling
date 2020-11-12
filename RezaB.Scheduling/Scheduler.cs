using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using System.Threading;

namespace RezaB.Scheduling
{
    public class Scheduler : IRezaBScheduler
    {
        public TimeSpan CheckIntervals { get; private set; }

        public string Name { get; private set; }

        public bool IsRunning { get { return _isRunning; } protected set { _isRunning = value; } }

        public IEnumerable<SchedulerOperation> Operations { get; private set; }

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

        public void Stop()
        {
            IsRunning = false;
            foreach (var operation in Operations)
            {
                operation.Stop();
            }
            mainSchedulerThread.Join();
        }

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
