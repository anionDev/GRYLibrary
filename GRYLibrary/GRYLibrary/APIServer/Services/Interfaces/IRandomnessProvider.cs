namespace GRYLibrary.Core.APIServer.Services.Interfaces
{
    public interface IRandomnessProvider
    {
        /// <returns>Returns a non-negative number which is lower than <paramref name="maximalValue"/>.</returns>
        public int Next(int maximalValue);
        void NextBytes(byte[] buffer);
    }
}
