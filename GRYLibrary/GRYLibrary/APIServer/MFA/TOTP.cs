namespace GRYLibrary.Core.APIServer.MFA
{
    public class TOTP : IMFAMethod
    {
        public string SecretKey { get; set; }
        public bool IsActicated { get; set; }
    }
}
