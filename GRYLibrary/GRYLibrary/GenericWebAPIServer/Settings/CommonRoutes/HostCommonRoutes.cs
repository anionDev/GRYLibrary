using Microsoft.AspNetCore.Mvc;

namespace GRYLibrary.Core.GenericWebAPIServer.Settings.CommonRoutes
{
    public class HostCommonRoutes :CommonRoutesInformation
    {
        public IActionResult TermsOfServiceRequestResult { get; set; } = new ContentResult() { StatusCode = 200, Content = "<html><body>TermsOfServiceRequestResult</body></html>", ContentType = "text/html" };// = new ContentResult() { StatusCode = (int)HttpStatusCode.NotFound };
        public IActionResult LicenseRequestResult { get; set; } = new ContentResult() { StatusCode = 200, Content = "<html><body>LicenseRequestResult</body></html>", ContentType = "text/html" };// = new ContentResult() { StatusCode = (int)HttpStatusCode.NotFound };
        public IActionResult ContactRequestResult { get; set; } = new ContentResult() { StatusCode = 200, Content = "<html><body>LicenseRequestResult</body></html>", ContentType = "text/html" };// = new ContentResult() { StatusCode = (int)HttpStatusCode.NotFound };
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
