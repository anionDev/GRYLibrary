using CommandLine;
using GRYLibrary.Core.Logging.GeneralPurposeLogger;

namespace GRYLibrary.Core.Miscellaneous.ConsoleApplication
{
    [Verb(nameof(Help), isDefault: false, HelpText = "Shows help")]
    public class Help : InternalCommand
    {

        public override int Run(GRYConsoleApplicationInitialInformation applicationInitialInformation)
        {
            this.ParserBase._Logger.Log($"{applicationInitialInformation.ProgramName} v{applicationInitialInformation.ProgramVersion}", Microsoft.Extensions.Logging.LogLevel.Information);
            if (applicationInitialInformation.ProgramDescription is not null)
            {
                this.ParserBase._Logger.Log(string.Empty, Microsoft.Extensions.Logging.LogLevel.Information);
                this.ParserBase._Logger.Log(applicationInitialInformation.ProgramDescription, Microsoft.Extensions.Logging.LogLevel.Information);
            }
            this.ParserBase._Logger.Log(string.Empty, Microsoft.Extensions.Logging.LogLevel.Information);
            this.ParserBase._Logger.Log("Available commands:", Microsoft.Extensions.Logging.LogLevel.Information);
            foreach (System.Type type in this.ParserBase.GetVerbs())
            {
                this.ParserBase._Logger.Log($"  - {type.Name.ToString().ToLower()}", Microsoft.Extensions.Logging.LogLevel.Information);
            }

            return 0;
        }
    }
}
