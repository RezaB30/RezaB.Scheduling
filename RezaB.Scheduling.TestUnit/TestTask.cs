using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RezaB.Scheduling.TestUnit
{
    class TestTask : AbortableTask
    {
        private int _maxLoopCount;
        private Random _rand;

        public TestTask(int maxLoopCount)
        {
            _maxLoopCount = maxLoopCount;
            _rand = new Random();
        }

        public override bool Run()
        {
            var loopCount = _rand.Next(1, _maxLoopCount);
            var waitTime = TimeSpan.FromSeconds(_rand.Next(1, 5));
            Console.WriteLine($"{Thread.CurrentThread.Name}>{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}-> demo task started with loop count {loopCount}.");
            for (int i = 0; i < loopCount; i++)
            {
                if(_isAborted)
                {
                    Console.WriteLine($"{Thread.CurrentThread.Name}>{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}-> aborted!");
                    break;
                }
                Console.WriteLine($"{Thread.CurrentThread.Name}>{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")}-> iteration {i}");
                Thread.Sleep(waitTime);
            }
            return true;
        }
    }
}
