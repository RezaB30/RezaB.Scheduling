using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RezaB.Scheduling;
using System.Threading;

namespace RezaB.Scheduling.TestUnit
{
    class Program
    {
        private static Random Rand = new Random();
        static void Main(string[] args)
        {
            var scheduleroperations = PrepareOperations();
            var scheduler = new Scheduler(scheduleroperations, TimeSpan.FromSeconds(10), "TestScheduler");
            Console.WriteLine($"Starting the {scheduler.Name} scheduler...");
            scheduler.Start();
            Console.WriteLine("Scheduler started.");
            Console.WriteLine("Press any key to stop it.");
            Console.ReadKey();
            scheduler.Stop();
            Console.WriteLine("Scheduler stopped");
            Console.WriteLine("Press any key to close...");
            Console.ReadKey();
        }

        private static List<SchedulerOperation> PrepareOperations()
        {
            return new List<SchedulerOperation>()
            {
                new SchedulerOperation("ChangeState", new TestTask(2), new StartParameters.SchedulerTimingOptions(new StartParameters.SchedulerWorkingTimeSpan(new TimeSpan(15,35,0), TimeSpan.FromDays(1).Subtract(TimeSpan.FromSeconds(1))), new StartParameters.SchedulerIntervalTimeSpan(TimeSpan.FromMinutes(2))), 1, new []
                {
                    new SchedulerTask("ChangeTariff", new TestTask(3), 2, new [] 
                    {
                        new SchedulerTask("IssueBills",new TestTask(6), 2, new[]
                        {
                            new SchedulerTask("IssueEBills", new TestTask(10), 4),
                            new SchedulerTask("AutoPayments", new TestTask(7), 3, new []
                            {
                                new SchedulerTask("CreateSMSes", new TestTask(3), 2)
                            })
                        })
                    }),
                }),
                new SchedulerOperation("SendSMS", new TestTask(12), new StartParameters.SchedulerTimingOptions(new StartParameters.SchedulerWorkingTimeSpan(TimeSpan.FromSeconds(1), TimeSpan.FromDays(1).Subtract(TimeSpan.FromSeconds(1)))), 1)
            };
        }
    }
}
