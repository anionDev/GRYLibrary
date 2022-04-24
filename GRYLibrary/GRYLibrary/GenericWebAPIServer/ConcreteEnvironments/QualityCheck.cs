namespace GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments
{
    public class QualityCheck : IEnvironment
    {
        #region Overhead
        public static QualityCheck Instance { get; } = new QualityCheck();
        private QualityCheck()
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
