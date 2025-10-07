using CommandLine;

namespace GRYLibrary.Core.APIServer.Verbs
{
    [Verb(nameof(RunServer), isDefault: true, HelpText = "Runs the server.")]
    public abstract class RunServer : IAPIServerCommandlineParameter
    {
        [Option(nameof(RealRun), Required = false, Default = false)]
        public bool RealRun { get; set; }
    }
}
