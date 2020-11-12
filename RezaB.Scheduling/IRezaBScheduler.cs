using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.Scheduling
{
    public interface IRezaBScheduler
    {
        void Start(string loggerName = null);

        void Stop();
    }
}
