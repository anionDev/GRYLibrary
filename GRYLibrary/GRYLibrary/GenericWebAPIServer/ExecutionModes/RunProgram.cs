using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.ExecutionModes
{
    public class RunProgram : ExecutionMode
    {
        public static RunProgram Instance { get; } = new RunProgram();
        private RunProgram() { }
        public override void Accept(IExecutionModeVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IExecutionModeVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
