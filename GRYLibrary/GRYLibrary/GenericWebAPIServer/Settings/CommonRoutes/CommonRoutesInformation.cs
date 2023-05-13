namespace GRYLibrary.Core.GenericWebAPIServer.Settings.CommonRoutes
{
    public abstract class CommonRoutesInformation
    {
        public abstract void Accept(ICommonRoutesInformationVisitor visitor);
        public abstract T Accept<T>(ICommonRoutesInformationVisitor<T> visitor);
    }
    public interface ICommonRoutesInformationVisitor
    {
        void Handle(DoNotHostCommonRoutes doNotHostCommonRoutes);
        void Handle(HostCommonRoutes hostCommonRoutes);
    }
    public interface ICommonRoutesInformationVisitor<T>
    {
        T Handle(DoNotHostCommonRoutes doNotHostCommonRoutes);
        T Handle(HostCommonRoutes hostCommonRoutes);
    }
}
