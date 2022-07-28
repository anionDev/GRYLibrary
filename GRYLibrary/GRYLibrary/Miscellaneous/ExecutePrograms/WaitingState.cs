using GRYLibrary.Core.Miscellaneous.ExecutePrograms.WaitingStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.ExecutePrograms
{
    public abstract class WaitingState
    {
        public abstract void Accept(IWaitingStateVisitor visitor);
        public abstract T Accept<T>(IWaitingStateVisitor<T> visitor);
    }
    public interface IWaitingStateVisitor
    {
        void Handle(RunAsynchronously runAsynchronously);
        void Handle(RunSynchronously runSynchronously);
    }
    public interface IWaitingStateVisitor<T>
    {
        T Handle(RunAsynchronously runAsynchronously);
        T Handle(RunSynchronously runSynchronously);
    }
}
