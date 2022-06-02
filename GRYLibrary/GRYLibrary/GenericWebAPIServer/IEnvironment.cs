using GRYLibrary.Core.GenericWebAPIServer.ConcreteEnvironments;

namespace GRYLibrary.Core.GenericWebAPIServer
{
    public interface IEnvironment
    {
        public void Accept(IEnvironmentVisitor visitor);
        public T Accept<T>(IEnvironmentVisitor<T> visitor);
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
