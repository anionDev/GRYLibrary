namespace GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments
{
    public class QualityCheck : Environment
    {
        public static QualityCheck Instance { get; } = new QualityCheck();
        private QualityCheck()
        {
        }
        public override void Accept(IEnvironmentVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(IEnvironmentVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
    }
}
