
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.Miscellaneous.ExecutePrograms.WaitingStates
{
    public class RunAsynchronously : WaitingState
    {
        public override void Accept(IWaitingStateVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IWaitingStateVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
