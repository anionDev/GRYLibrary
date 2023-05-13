using CommandLine;
using CommandLine.Text;
using GRYLibrary.Core.GenericWebAPIServer.ExecutionModes;
using GRYLibrary.Core.Log;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;

namespace GRYLibrary.Core.Miscellaneous.ConsoleApplication
{
    public class GRYConsoleApplication<CMDOptions, InitializationConfig> where CMDOptions : ICommandlineParameter
    {
        private readonly Func<CMDOptions, InitializationConfig, int> _Main;
        private readonly string _ProgramName;
        private readonly string _ProgramVersion;
        private readonly string _ProgramDescription;
        private readonly GRYLog _Log;
        private readonly ExecutionMode _ExecutionMode;
        private readonly SentenceBuilder _SentenceBuilder;
        private readonly bool _ProgramCanRunWithoutArguments;
        public GRYConsoleApplication(Func<CMDOptions,  InitializationConfig, int> main, string programName, string programVersion, string programDescription, bool programCanRunWithoutArguments, ExecutionMode executionMode)
        {
            this._Main = main;
            this._ProgramName = programName;
            this._ProgramVersion = programVersion;
            this._ProgramDescription = programDescription;
            this._Log = GRYLog.Create();
            this._SentenceBuilder = SentenceBuilder.Create();
            this._ProgramCanRunWithoutArguments = programCanRunWithoutArguments;
            this._ExecutionMode = executionMode;
        }

        public int Main(string[] arguments, InitializationConfig initializationConfiguration)
        {
            int result = 1;
            try
            {
                string title = $"{this._ProgramName} (v{this._ProgramVersion})";
                Console.Title = title;
                if(arguments == null)
                {
                    throw Utilities.CreateNullReferenceExceptionDueToParameter(nameof(arguments));
                }
                string argumentsAsString = string.Join(' ', arguments);
                string workingDirectory = Directory.GetCurrentDirectory();
                try
                {
                    if(this._ExecutionMode is Analysis)
                    {
                        arguments = Array.Empty<string>();
                    }
                    if(arguments.Length == 0 && !this._ProgramCanRunWithoutArguments)
                    {
                        this._Log.Log($"{this._ProgramName} v{this._ProgramVersion}");
                        this._Log.Log($"Run '{this._ProgramName} --help' to get help about the usage.");
                    }
                    else
                    {
                        ParserResult<CMDOptions> parserResult = new Parser(settings => settings.CaseInsensitiveEnumValues = true).ParseArguments<CMDOptions>(arguments);
                        if(ShowHelp(arguments))
                        {
                            this.WriteHelp(parserResult);
                            result = 0;
                        }
                        else
                        {
                            parserResult
                                .WithParsed(options =>
                                {
                                    options.OriginalArguments = arguments;
                                    result = this.HandleSuccessfullyParsedArguments(options, initializationConfiguration);
                                })
                                .WithNotParsed(errors =>
                                {
                                    result = 3;
                                    this.HandleParsingErrors(argumentsAsString, errors);
                                });
                        }
                    }
                }
                catch(Exception exception)
                {
                    this._Log.Log($"Fatal error occurred while processing argument.", exception);
                }
            }
            catch(Exception exception)
            {
                this._Log.Log($"Fatal error occurred", exception);
                result = 2;
            }
            this._Log.Log($"Finished program", LogLevel.Debug);
            return result;
        }

        private void HandleParsingErrors(string argumentsAsString, IEnumerable<Error> errors)
        {
            int amountOfErrors = errors.Count();
            this._Log.Log($"Argument '{argumentsAsString}' could not be parsed successfully.", LogLevel.Error);
            if(0 < amountOfErrors)
            {
                this._Log.Log($"The following error{(amountOfErrors == 1 ? string.Empty : "s")} occurred:", LogLevel.Error);
                foreach(Error error in errors)
                {
                    this._Log.Log($"{error.Tag}: {this._SentenceBuilder.FormatError(error)}", LogLevel.Error);
                }
            }
        }

        private int HandleSuccessfullyParsedArguments(CMDOptions options, InitializationConfig gc)
        {
            return this._Main(options,  gc);
        }

        public void WriteHelp(ParserResult<CMDOptions> argumentParserResult)
        {
            this._Log.Log(HelpText.AutoBuild(argumentParserResult).ToString());
            if(this._ProgramDescription is not null)
            {
                this._Log.Log(string.Empty);
                this._Log.Log(this._ProgramDescription);
            }
        }

        public static bool ShowHelp(string[] commandlineArguments)
        {
            if(commandlineArguments.Length == 0)
            {
                return false;
            }
            else
            {
                string argumentLower = commandlineArguments[0].ToLower();
                return argumentLower.Equals("--help")
                    || argumentLower.Equals("-h");
            }
        }
    }
}
