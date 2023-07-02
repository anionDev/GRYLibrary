namespace GRYLibrary.Core.GenericWebAPIServer.CommonRoutes
{
    public class HostCommonRoutes :CommonRoutesHostInformation
    {
        #region Overhead
        public override void Accept(ICommonRoutesHostInformationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(ICommonRoutesHostInformationVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
        #endregion
    }
}
