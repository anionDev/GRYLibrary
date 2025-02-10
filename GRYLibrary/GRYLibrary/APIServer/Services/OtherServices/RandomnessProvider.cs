using GRYLibrary.Core.APIServer.Services.Interfaces;
using System;

namespace GRYLibrary.Core.APIServer.Services.OtherServices
{
    public class RandomnessProvider : IRandomnessProvider
    {
        private readonly Random _Random;
        public RandomnessProvider(Random random)
        {
            this._Random = random;
        }
        public int Next(int maximalValue)
        {
            return this._Random.Next(maximalValue);
        }

        public void NextBytes(byte[] buffer)
        {
            this._Random.NextBytes(buffer);
        }
    }
}
