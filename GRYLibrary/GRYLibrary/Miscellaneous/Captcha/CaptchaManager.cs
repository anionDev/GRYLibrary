using System;
using System.Collections.Concurrent;

namespace GRYLibrary.Core.Miscellaneous.Captcha
{

    public class CaptchaManager
    {
        private readonly ConcurrentDictionary<string, CaptchaInstance> _Captchas = new ConcurrentDictionary<string, CaptchaInstance>();
        private readonly ConcurrentDictionary<string, DateTime> _AccessKeys = new ConcurrentDictionary<string, DateTime>();
        public static CaptchaManager Instance { get; } = new CaptchaManager();

        private CaptchaManager()
        {
            //TODO start cleanup service to remove expired things
        }
        public CaptchaInstance GetNewCaptcha(CaptchaGenerationSettings settings)
        {
            CaptchaInstance result = new CaptchaInstance(settings);
            if(this._Captchas.TryAdd(result.Id, result))
            {
                return result;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        internal static DateTime GetCurrentTime()
        {
            return DateTime.Now;
        }

        public bool TrySolve(string captchaId, string userInput, out string accessKey, out string failMessage)
        {
            if(this._Captchas.ContainsKey(captchaId))
            {
                CaptchaInstance captcha = this._Captchas[captchaId];
                if(captcha.ExpectedUserInput == userInput)
                {
                    DateTime now = GetCurrentTime();
                    if(now < captcha.ValidUntil)
                    {
                        failMessage = null;
                        accessKey = Guid.NewGuid().ToString();
                        if(this._AccessKeys.TryAdd(accessKey, captcha.AccessTokenValidUntil))
                        {
                            return true;
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        failMessage = "Captcha expired";
                    }
                }
                else
                {
                    failMessage = "Wrong captcha-text";
                }
            }
            else
            {
                failMessage = "Unknown cpatcha";
            }
            accessKey = null;
            return false;
        }

        internal bool UserHasAlreadySolvedTheCaptcha(string accessToken, out string failMessage)
        {
            failMessage = null;
            if(accessToken is null)
            {
                failMessage = "No accesstoken provided";
            }
            else
            {
                if(this._AccessKeys.ContainsKey(accessToken))
                {
                    if(this._AccessKeys.TryGetValue(accessToken, out DateTime validUntil))
                    {
                        DateTime now = GetCurrentTime();
                        if(now < validUntil)
                        {
                            return true;
                        }
                        else
                        {
                            failMessage = "Accesstoken expired";
                        }
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    failMessage = "Unknown accesstoken";
                }
            }
            return false;
        }
    }
}
