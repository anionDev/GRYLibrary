using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public abstract class Environment
    {
        private protected Environment() { }
        public abstract void Accept(IEnvironmentVisitor visitor);
        public abstract T Accept<T>(IEnvironmentVisitor<T> visitor);
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            else
            {
                return this.GetType().Equals(obj.GetType());
            }
        }
        public override int GetHashCode()
        {
            return this.GetType().GetHashCode();
        }
    }
    public interface IEnvironmentVisitor
    {
        void Handle(Development environment);
        void Handle(Productive environment);
        void Handle(QualityCheck environment);
    }
    public interface IEnvironmentVisitor<T>
    {
        T Handle(Development environment);
        T Handle(Productive environment);
        T Handle(QualityCheck environment);
    }
}
