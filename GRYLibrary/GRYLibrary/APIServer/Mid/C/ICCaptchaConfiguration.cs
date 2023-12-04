using GRYLibrary.Core.APIServer.MidT.Captcha;
using System;

namespace GRYLibrary.Core.APIServer.Mid.C
{
    public interface ICCaptchaConfiguration : ICaptchaConfiguration
    {
        public string Encoding { get; set; }
        public ushort Length { get; set; }
        public string Alphabet { get; set; }
        public TimeSpan ExpireDurationOfCaptcha { get; set; }
        public TimeSpan ExpireDurationOfAccessToken { get; set; }
        public string CaptchaCookieName { get; set; }
        public string CaptchaPage { get; set; }
    }
}
