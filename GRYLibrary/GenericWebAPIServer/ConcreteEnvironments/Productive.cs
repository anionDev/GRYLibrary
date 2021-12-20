namespace GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments
{
    public class Productive : IEnvironment
    {
        #region Overhead
        public static Productive Instance { get; } = new Productive();
        private Productive()
        {
        }
        public void Accept(IEnvironmentVisitor visitor)
        {
            visitor.Handle(this);
        }

        public T Accept<T>(IEnvironmentVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
        #endregion
    }
}
