﻿using GRYLibrary.Core.ExecutePrograms;
using System;

namespace GRYLibrary.Core.Exceptions
{
    public class UnexpectedExitCodeException : Exception
    {
        public ExternalProgramExecutor ExecutedProgram { get; }
        public UnexpectedExitCodeException(ExternalProgramExecutor externalProgramExecutor) : base(GetMessage(externalProgramExecutor))
        {
            this.ExecutedProgram = externalProgramExecutor;
        }

        private static string GetMessage(ExternalProgramExecutor externalProgramExecutor) => $"'{externalProgramExecutor.Configuration.Title}' had exitcode {externalProgramExecutor.ExitCode}.{Environment.NewLine}{Environment.NewLine}{externalProgramExecutor.GetSummaryOfExecutedProgram()}";
    }
}