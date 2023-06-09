namespace GRYLibrary.Core.GenericWebAPIServer.DefinedMiddlewares.Captcha
{
    public interface ISupportCaptchaMiddleware :ISupportedMiddleware
    {
        ICaptchaConfiguration ConfigurationForCaptchaMiddleware { get; set; }
    }
}
