using GRYLibrary.Core.Misc;

namespace GRYLibrary.Core.Crypto
{
    public class SHA256 : HashAlgorithm
    {
        public override byte[] Hash(byte[] data) => System.Security.Cryptography.SHA256.HashData(data);
        public override byte[] GetIdentifier() => Utilities.PadLeft(System.Text.Encoding.ASCII.GetBytes(nameof(SHA256)), 10);
    }
}