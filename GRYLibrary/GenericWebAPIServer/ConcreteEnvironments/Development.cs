namespace GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments
{
    public class Development : IEnvironment
    {
        #region Overhead
        public static Development Instance { get; } = new Development();
        private Development()
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
