using CommandLine;
using CommandLine.Text;
using GRYLibrary.Core.ExecutePrograms;
using GRYLibrary.Core.ExecutePrograms.WaitingStates;
using GRYLibrary.Core.Log;
using GRYLibrary.Core.Log.ConcreteLogTargets;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;

namespace GRYLibrary.Core.Miscellaneous
{
    public class GRYConsoleApplication<T>
    {
        private readonly Func<T, int> _Main;
        private readonly string _ProgramName;
        private readonly string _ProgramVersion;
        private readonly string _ProgramDescription;
        private readonly GRYLog _Log;
        private readonly SentenceBuilder _SentenceBuilder;
        public GRYConsoleApplication(Func<T, int> main, string programName, string programVersion, string programDescription)
        {
            this._Main = main;
            this._ProgramName = programName;
            this._ProgramVersion = programVersion;
            this._ProgramDescription = programDescription;
            this._Log = GRYLog.Create();
            _SentenceBuilder = SentenceBuilder.Create();
        }

        public int Main(string[] arguments)
        {
            int result = 2;
            try
            {
                if(arguments == null)
                {
                    throw Utilities.CreateNullReferenceExceptionDueToParameter(nameof(arguments));
                }
                string argumentsAsString = String.Join(' ', arguments);
                string workingDirectory = Directory.GetCurrentDirectory();
                try
                {
                    if(arguments.Length == 0)
                    {
                        _Log.Log($"{_ProgramName} v{_ProgramVersion}");
                        _Log.Log($"Run '{_ProgramName} --help' to get help about the usage.");
                    }
                    else
                    {
                        ParserResult<T> parserResult = new CommandLine.Parser(settings => settings.CaseInsensitiveEnumValues = true).ParseArguments<T>(arguments);
                        if(ShowHelp(arguments))
                        {
                            WriteHelp(parserResult);
                        }
                        else
                        {
                            parserResult.WithParsed(options =>
                            {
                                result = HandleSuccessfullyParsedArguments(options);
                            })
                            .WithNotParsed(errors =>
                            {
                                HandleParsingErrors(argumentsAsString, errors);
                            });
                        }
                    }
                }
                catch(Exception exception)
                {
                    _Log.Log($"Fatal error occurred while processing argument '{workingDirectory}> epew {argumentsAsString}", exception);
                }
            }
            catch(Exception exception)
            {
                _Log.Log($"Fatal error occurred", exception);
                result = 1;
            }
            _Log.Log($"Finished Epew", LogLevel.Debug);
            return result;
        }


        private void HandleParsingErrors(string argumentsAsString, IEnumerable<Error> errors)
        {
            var amountOfErrors = errors.Count();
            _Log.Log($"Argument '{argumentsAsString}' could not be parsed successfully.", Microsoft.Extensions.Logging.LogLevel.Error);
            if(0 < amountOfErrors)
            {
                _Log.Log($"The following error{(amountOfErrors == 1 ? string.Empty : "s")} occurred:", Microsoft.Extensions.Logging.LogLevel.Error);
                foreach(var error in errors)
                {
                    _Log.Log($"{error.Tag}: {_SentenceBuilder.FormatError(error)}", Microsoft.Extensions.Logging.LogLevel.Error);
                }
            }
        }

        private int HandleSuccessfullyParsedArguments(T options)
        {
            return _Main(options);
        }

        public void WriteHelp(ParserResult<T> argumentParserResult)
        {
            _Log.Log(HelpText.AutoBuild(argumentParserResult).ToString());
            if(_ProgramDescription is not null)
            {
                _Log.Log(string.Empty);
                _Log.Log(_ProgramDescription);
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
