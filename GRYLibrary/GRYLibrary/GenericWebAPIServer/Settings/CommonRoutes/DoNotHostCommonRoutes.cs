namespace GRYLibrary.Core.GenericWebAPIServer.Settings.CommonRoutes
{
    public class DoNotHostCommonRoutes :CommonRoutesInformation
    {
        #region Overhead
        public override void Accept(ICommonRoutesInformationVisitor visitor)
        {
            visitor.Handle(this);
        }

        public override T Accept<T>(ICommonRoutesInformationVisitor<T> visitor)
        {
            return visitor.Handle(this);
        }
        #endregion
    }
}
