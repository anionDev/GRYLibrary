namespace GRYLibrary.Core.Miscellaneous.ConsoleApplication
{
    public abstract class InternalCommand : ICommandlineParameter
    {
        public ParserBase ParserBase { get; internal set; }
        public abstract int Run(GRYConsoleApplicationInitialInformation applicationInitialInformation);
    }
}
