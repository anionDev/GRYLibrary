using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;

namespace GRYLibrary.Core.APIServer.Mid.Captcha
{
    public class CaptchaConfiguration : ICaptchaConfiguration
    {
        public bool Enabled { get; set; } = true;
        public ushort Length { get; set; } = 8;
        public string Alphabet { get; set; } = "ABDEFGHKLMNPQRSTUVYabdefghjkmnpqrstuvy23456789";
        public TimeSpan ExpireDurationOfCaptcha { get; set; } = TimeSpan.FromMinutes(1);
        public TimeSpan ExpireDurationOfAccessToken { get; set; } = TimeSpan.FromDays(1);
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
        __[message]__</br>
    <img src=""data:image/jpeg;base64, __[captchabase64]__"" />
    <form action="""">
        <button type=""submit"">↻</button>
    </form>
        <form>
            <label for=""Value""> Value:</label> <br />
            <input type=""text"" name=""__[captchavaluequerykey]__"" required> <br />
            <input type=""hidden"" name=""__[captchaidquerykey]__"" value=""__[captchaid]__"" required> <br />
            <input type=""submit"" value=""Enter"">
        </form>
    </div>
</body>
</html>";

        public ISet<FilterDescriptor> GetFilter()
        {
            return new HashSet<FilterDescriptor>();
        }
    }
}
