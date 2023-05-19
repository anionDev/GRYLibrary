using System;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces
{
    public interface ICaptchaMiddlewareSettings :IMiddlewareSettings
    {
        public string Encoding { get; set; }
        public ushort Length { get; set; } 
        public string Alphabet { get; set; }
        public TimeSpan ExpireDurationOfCaptcha { get; set; }
        public TimeSpan ExpireDurationOfAccessToken { get; set; }
        public string CaptchaCookieName { get; set; }
        public string CaptchaPage { get; set; }

        public bool TrySolve(string captchaId, string userInput, out string accessKey, out string failMessage);
        public (string id,byte[] picture) GenerateCaptcha();
        bool UserHasAlreadySolvedTheCaptcha(string accessToken, out string failMessage);
    }
}
