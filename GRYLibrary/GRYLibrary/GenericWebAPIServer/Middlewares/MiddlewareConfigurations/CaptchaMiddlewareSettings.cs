using GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurationInterfaces;
using GRYLibrary.Core.Miscellaneous.Captcha;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.GenericWebAPIServer.Middlewares.MiddlewareConfigurations
{
    public class CaptchaMiddlewareSettings :ICaptchaMiddlewareSettings
    {
        private readonly CaptchaManager _CaptchaManager;
        public bool Enabled { get; set; } = true;
        public ushort Length { get; set; } = 8;
        public string Alphabet { get; set; } = "ABDEFGHKLMNPQRSTUVYabdefghjkmnpqrstuvy23456789";
        public TimeSpan ExpireDurationOfCaptcha { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan ExpireDurationOfAccessToken { get; set; } = TimeSpan.FromHours(1);
        public string Encoding { get; set; } = "utf-8";
        public string CaptchaCookieName { get; set; } = "Captcha";
        public string CaptchaPage { get; set; } = @$"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"">
</head>
<body>
    <style>
    </style>
    <div>
        __message__</br>
    <img src=""data:image/jpeg;base64, __captchabase64__"" />
    <form action="""">
        <button type=""submit"">↻</button>
    </form>
        <form>
            <label for=""Value""> Value:</label> <br />
            <input type=""text"" name=""__captchavaluequerykey__"" required> <br />
            <input type=""hidden"" name=""__captchaidquerykey__"" value=""__captchaid__"" required> <br />
            <input type=""submit"" value=""Enter"">
        </form>
    </div>
</body>
</html>";

        public CaptchaMiddlewareSettings()
        {
            this._CaptchaManager = CaptchaManager.Instance;
        }
        public bool TrySolve(string captchaId, string userInput, out string accessKey, out string failMessage)
        {
            return this._CaptchaManager.TrySolve(captchaId, userInput, out accessKey, out failMessage);
        }
        public (string id, byte[] picture) GenerateCaptcha()
        {
            CaptchaInstance result = this._CaptchaManager.GetNewCaptcha(this.GetCaptchaSettings());
            return (result.Id, result.PictureContent);
        }

        private CaptchaGenerationSettings GetCaptchaSettings()
        {
            CaptchaGenerationSettings result = new CaptchaGenerationSettings()
            {
                Alphabet = Alphabet,
                ExpireDurationOfCaptcha = ExpireDurationOfCaptcha,
                Length = Length,
                ExpireDurationOfAccessToken = ExpireDurationOfAccessToken
            };
            return result;
        }

        public bool UserHasAlreadySolvedTheCaptcha(string accessToken, out string failMessage)
        {
            return this._CaptchaManager.UserHasAlreadySolvedTheCaptcha(accessToken, out failMessage);
        }
        public ISet<IOperationFilter> GetFilter()
        {
            return new HashSet<IOperationFilter>();
        }
    }
}
