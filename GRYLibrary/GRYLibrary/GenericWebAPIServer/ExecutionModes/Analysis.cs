using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.ExecutionModes
{
    public class Analysis: ExecutionMode
    {
        public static Analysis Instance { get; } = new Analysis();
        private Analysis() { }

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
