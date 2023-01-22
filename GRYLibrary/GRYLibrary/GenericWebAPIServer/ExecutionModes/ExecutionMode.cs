namespace GRYLibrary.Core.GenericWebAPIServer.ExecutionModes
{
    public abstract class ExecutionMode
    {
        public abstract void Accept(IExecutionModeVisitor visitor);
        public abstract T Accept<T>(IExecutionModeVisitor<T> visitor);
    }
    public interface IExecutionModeVisitor
    {
        void Handle(Analysis analysis);
        void Handle(RunProgram runProgram);
    }
    public interface IExecutionModeVisitor<T>
    {
        T Handle(Analysis analysis);
        T Handle(RunProgram runProgram);
    }
}
