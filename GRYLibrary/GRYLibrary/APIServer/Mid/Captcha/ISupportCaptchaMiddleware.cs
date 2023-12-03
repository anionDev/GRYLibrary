namespace GRYLibrary.Core.APIServer.Mid.Captcha
{
    public interface ISupportCaptchaMiddleware : ISupportedMiddleware
    {
        ICaptchaConfiguration ConfigurationForCaptchaMiddleware { get; }
    }
}
