using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RezaB.Scheduling
{
    public abstract class AbortableTask
    {
        protected volatile bool _isAborted = false;
        //private Func<bool> _actionMethod;

        //public bool Run() { return _actionMethod(); }

        public void Abort() { _isAborted = true; }

        //public AbortableTask(Func<bool> actionMethod)
        //{
        //    _actionMethod = actionMethod;
        //}

        public abstract bool Run();
    }
}
