using CommandLine;
using CommandLine.Text;
using GRYLibrary.Core.APIServer.ConcreteEnvironments;
using GRYLibrary.Core.APIServer.ExecutionModes;
using Microsoft.Extensions.Logging;
using System;
using GUtilities = GRYLibrary.Core.Miscellaneous.Utilities;
using System.Collections.Generic;
using System.IO;
using GRYLibrary.Core.Logging.GRYLogger;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;

namespace GRYLibrary.Core.Miscellaneous.ConsoleApplication
{
    public abstract class ParserBase
    {
        public string[] OriginalArguments { get; internal set; }
        public string OriginalArgumentsAsString { get; internal set; }
        internal IGeneralLogger _Logger;
        internal SentenceBuilder _SentenceBuilder;
        internal GRYConsoleApplicationInitialInformation ApplicationInitialInformation;
        protected abstract int RunImplementation(ParserResult<object> parsed);
        public int Run(ParserResult<object> parsed)
        {
            //TODO set variables;
            return this.RunImplementation(parsed);
        }
        protected Func<IEnumerable<Error>, int> Error(string argumentsAsString)
        {
            return errors =>
            {
                int amountOfErrors = errors.Count();
                this._Logger.Log($"Argument '{argumentsAsString}' could not be parsed successfully.", LogLevel.Error);
                if (0 < amountOfErrors)
                {
                    this._Logger.Log($"The following error{(amountOfErrors == 1 ? string.Empty : "s")} occurred:", LogLevel.Error);
                    foreach (Error error in errors)
                    {
                        this._Logger.Log($"{error.Tag}: {this._SentenceBuilder.FormatError(error)}", LogLevel.Error);
                    }
                }
                return 1;
            };
        }
    }
    public class ParserWithoutArguments : ParserBase
    {
        private readonly Func<int> _Verb00;
        public ParserWithoutArguments(Func<int> verb00)
        {
            this._Verb00 = verb00;
        }

        public int RunVerb00()
        {
            return this._Verb00();
        }

        protected override int RunImplementation(ParserResult<object> parsed)
        {
            throw new NotImplementedException();
        }
    }
    public class ParserWithoutVerbs<Options> : ParserBase
    {
        private readonly Func<Options, GRYConsoleApplicationInitialInformation, int> _Runner;
        public ParserWithoutVerbs(Func<Options, GRYConsoleApplicationInitialInformation, int> verb00)
        {
            this._Runner = verb00;
        }

        protected override int RunImplementation(ParserResult<object> parsed)
        {
            return parsed.MapResult((Options options) => this._Runner(options,ApplicationInitialInformation),
                                    this.Error(this.OriginalArgumentsAsString));
        }
    }
    public class VerbParser<Verb01> : ParserBase
    {
        private readonly Func<Verb01, GRYConsoleApplicationInitialInformation, int> _Verb01Runner;
        public VerbParser(Func<Verb01, GRYConsoleApplicationInitialInformation, int> verb01Runner)
        {
            this._Verb01Runner = verb01Runner;
        }

        protected override int RunImplementation(ParserResult<object> parsed)
        {
            return parsed.MapResult((Verb01 options) => this._Verb01Runner(options, ApplicationInitialInformation),
                                    this.Error(this.OriginalArgumentsAsString));
        }
    }
    public class VerbParser<Verb01, Verb02> : ParserBase
    {
        private readonly Func<Verb01, GRYConsoleApplicationInitialInformation, int> _Verb01Runner;
        private readonly Func<Verb02, GRYConsoleApplicationInitialInformation, int> _Verb02Runner;
        public VerbParser(Func<Verb01, GRYConsoleApplicationInitialInformation, int> verb01Runner, Func<Verb02, GRYConsoleApplicationInitialInformation, int> verb02Runner)
        {
            this._Verb01Runner = verb01Runner;
            this._Verb02Runner = verb02Runner;
        }

        protected override int RunImplementation(ParserResult<object> parsed)
        {
            return parsed.MapResult((Verb01 options) => this._Verb01Runner(options, ApplicationInitialInformation),
                                    (Verb02 options) => this._Verb02Runner(options, ApplicationInitialInformation),
                                    this.Error(this.OriginalArgumentsAsString));
        }
    }
    public class VerbParser<Verb01, Verb02, Verb03> : ParserBase
    {
        private readonly Func<Verb01, GRYConsoleApplicationInitialInformation, int> _Verb01Runner;
        private readonly Func<Verb02, GRYConsoleApplicationInitialInformation, int> _Verb02Runner;
        private readonly Func<Verb03, GRYConsoleApplicationInitialInformation, int> _Verb03Runner;
        public VerbParser(Func<Verb01, GRYConsoleApplicationInitialInformation, int> verb01Runner, Func<Verb02, GRYConsoleApplicationInitialInformation, int> verb02Runner, Func<Verb03, GRYConsoleApplicationInitialInformation, int> verb03Runner)
        {
            this._Verb01Runner = verb01Runner;
            this._Verb02Runner = verb02Runner;
            this._Verb03Runner = verb03Runner;
        }

        protected override int RunImplementation(ParserResult<object> parsed)
        {
            return parsed.MapResult((Verb01 options) => this._Verb01Runner(options, ApplicationInitialInformation),
                                    (Verb02 options) => this._Verb02Runner(options, ApplicationInitialInformation),
                                    (Verb03 options) => this._Verb03Runner(options, ApplicationInitialInformation),
                                    this.Error(this.OriginalArgumentsAsString));
        }
    }
    public class GRYConsoleApplication<CMDOptions, InitializationConfig> where CMDOptions : ICommandlineParameter
    {
        private readonly ParserBase _Mains;
        private readonly string _ProgramName;
        private readonly string _ProgramVersion;
        private readonly string _ProgramDescription;
        private readonly GRYLog _Log;
        private readonly ExecutionMode _ExecutionMode;
        private readonly SentenceBuilder _SentenceBuilder;
        private readonly bool _ProgramCanRunWithoutArguments;
        private readonly GRYConsoleApplicationInitialInformation _GRYConsoleApplicationInitialInformation;
        private readonly bool _ResetConsoleToDefaultvalues;
        public GRYConsoleApplication(ParserBase mains, string programName, string programVersion, string programDescription, bool programCanRunWithoutArguments, ExecutionMode executionMode, GRYEnvironment environment, bool resetConsoleToDefaultvalues)
        {
            this._Mains = mains;
            this._ProgramName = programName;
            this._ProgramVersion = programVersion;
            this._ProgramDescription = programDescription;
            this._Log = GRYLog.Create();
            this._SentenceBuilder = SentenceBuilder.Create();
            this._ProgramCanRunWithoutArguments = programCanRunWithoutArguments;
            this._ExecutionMode = executionMode;
            this._ResetConsoleToDefaultvalues = resetConsoleToDefaultvalues;
            this._GRYConsoleApplicationInitialInformation = new GRYConsoleApplicationInitialInformation(this._ProgramName, this._ProgramVersion, this._ProgramDescription, this._ExecutionMode, environment);
        }

        public int Main(string[] arguments)
        {
            int result = 1;
            try
            {
                try
                {
                    if (this._ResetConsoleToDefaultvalues)
                    {
                        Console.Clear();
                        if (GUtilities.DarkModeEnabled)
                        {
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        else
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }
                    }
                }
                catch
                {
                    GUtilities.NoOperation();
                }
                string title = $"{this._ProgramName} (v{this._ProgramVersion})";
                Console.Title = title;
                if (arguments == null)
                {
                    throw GUtilities.CreateNullReferenceExceptionDueToParameter(nameof(arguments));
                }
                string argumentsAsString = string.Join(' ', arguments);
                string workingDirectory = Directory.GetCurrentDirectory();
                try
                {
                    this._Log.Log($"Arguments: \"{argumentsAsString}\"", LogLevel.Debug);
                    if (this._ExecutionMode is Analysis)
                    {
                        arguments = Array.Empty<string>();
                    }
                    if (arguments.Length == 0 && !this._ProgramCanRunWithoutArguments)
                    {
                        this._Log.Log($"{this._ProgramName} v{this._ProgramVersion}");
                        this._Log.Log($"Run '{this._ProgramName} --help' to get help about the usage.");
                    }
                    else
                    {
                        ParserResult<CMDOptions> parserResult = new Parser(settings => settings.CaseInsensitiveEnumValues = true).ParseArguments<CMDOptions>(arguments);
                        if (ShowHelp(arguments))
                        {
                            this.WriteHelp(parserResult);
                            result = 0;
                        }
                        else
                        {
                            /*
                                    result = this.HandleSuccessfullyParsedArguments(options, initializationConfiguration);
                            */
                            ParserResult<object> parsed = Parser.Default.ParseArguments(arguments);
                            _Mains.OriginalArguments = arguments;
                            _Mains.OriginalArgumentsAsString = argumentsAsString;
                            _Mains._Logger = _Log;
                            _Mains._SentenceBuilder = _SentenceBuilder;
                            _Mains.ApplicationInitialInformation = _GRYConsoleApplicationInitialInformation;
                            return this._Mains.Run(parsed);
                        }
                    }
                }
                catch (Exception exception)
                {
                    this._Log.Log($"Fatal error occurred while processing argument.", exception);
                }
            }
            catch (Exception exception)
            {
                this._Log.Log($"Fatal error occurred", exception);
                result = 2;
            }
            this._Log.Log($"Finished program", LogLevel.Debug);
            return result;
        }

        public void WriteHelp(ParserResult<CMDOptions> argumentParserResult)
        {
            this._Log.Log(HelpText.AutoBuild(argumentParserResult).ToString());
            if (this._ProgramDescription is not null)
            {
                this._Log.Log(string.Empty);
                this._Log.Log(this._ProgramDescription);
            }
        }

        public static bool ShowHelp(string[] commandlineArguments)
        {
            if (commandlineArguments.Length != 1)
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
