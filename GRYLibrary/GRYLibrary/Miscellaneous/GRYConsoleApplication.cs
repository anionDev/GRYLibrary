using GRYLibrary.Core.Log;
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
        private readonly Func<string[], int> _Main;

        public GRYConsoleApplication(Func<string[], int> main)
        {
            this._Main = main;
        }

        public int Main(string[] arguments)
        {
            int result = ExitCodeNoProgramExecuted;
            try
            {
                Version = GetVersion();
                LicenseLink = $"https://raw.githubusercontent.com/anionDev/Epew/v{Version}/License.txt";
                _SentenceBuilder = SentenceBuilder.Create();
                if(arguments == null)
                {
                    throw Utilities.CreateNullReferenceExceptionDueToParameter(nameof(arguments));
                }
                string argumentsAsString = String.Join(' ', arguments);
                _Log = GRYLog.Create();
                string workingDirectory = Directory.GetCurrentDirectory();
                try
                {
                    if(arguments.Length == 0)
                    {
                        _Log.Log($"{ProgramName} v{Version}");
                        _Log.Log($"Run '{ProgramName} --help' to get help about the usage.");
                    }
                    else
                    {
                        ParserResult<T> parserResult = new Parser(settings => settings.CaseInsensitiveEnumValues = true).ParseArguments<T>(arguments);
                        if(IsHelpCommand(arguments))
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
                result = ExitCodeFatalErroroccurred;
            }
            _Log.Log($"Finished Epew", LogLevel.Debug);
            return result;
        }

    }
}
