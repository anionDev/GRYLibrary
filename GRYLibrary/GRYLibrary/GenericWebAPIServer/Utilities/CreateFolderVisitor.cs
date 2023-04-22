using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GRYLibrary.Core.GenericWebAPIServer.Utilities
{
    public class CreateFolderVisitor :IExecutionModeVisitor
    {
        private readonly string[] _Folder;

        public CreateFolderVisitor(params string[] folder)
        {
            this._Folder = folder;
        }

        public void Handle(Analysis analysis)
        {
            Miscellaneous.Utilities.NoOperation();
        }

        public void Handle(RunProgram runProgram)
        {
            foreach(string folder in this._Folder)
            {
                Miscellaneous.Utilities.EnsureDirectoryExists(folder);
            }
        }
    }
}
