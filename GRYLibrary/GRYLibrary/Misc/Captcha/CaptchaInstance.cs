using DNTCaptcha.Core;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace GRYLibrary.Core.Misc.Captcha
{

    public class CaptchaInstance
    {
        private static readonly Random _Random = new Random();
        public string Id { get; set; }
        public string ExpectedUserInput { get; set; }
        public byte[] PictureContent { get; set; }
        public DateTime ValidUntil { get; set; }
        public DateTime AccessTokenValidUntil { get; set; }
        public CaptchaInstance(CaptchaGenerationSettings settings)
        {
            this.Id = Guid.NewGuid().ToString();
            this.ExpectedUserInput = GetNewRandomExpectedUserInput(settings);
            this.PictureContent = GetPictureForString(this.ExpectedUserInput);
            this.ValidUntil = CaptchaManager.GetCurrentTime().Add(settings.ExpireDurationOfCaptcha);
            this.AccessTokenValidUntil = CaptchaManager.GetCurrentTime().Add(settings.ExpireDurationOfAccessToken);
        }

        internal static string GetNewRandomExpectedUserInput(CaptchaGenerationSettings settings)
        {
            return new string(Enumerable.Repeat(settings.Alphabet, settings.Length).Select(s => s[_Random.Next(s.Length)]).ToArray());
        }
        internal static byte[] GetPictureForString(string expectedUserInput)
        {
            RandomNumberProvider rng = new RandomNumberProvider();
            IOptions<DNTCaptchaOptions> options = Options.Create(new DNTCaptchaOptions());
            //TODO use settings
            CaptchaImageProvider imageProvider = new CaptchaImageProvider(rng, options);
            return imageProvider.DrawCaptcha(expectedUserInput, "black", "white", 25, "Tahoma");
        }
    }
}
