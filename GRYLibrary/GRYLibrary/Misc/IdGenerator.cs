using System;
using System.Collections.Generic;
using System.Numerics;

namespace GRYLibrary.Core.Misc
{
    /// <summary>
    /// Simple generic generator for ids.
    /// </summary>
    /// <typeparam name="T">Represents the type of an id</typeparam>
    public interface IIdGenerator<T>
    {
        public T GenerateNewId();
        public ISet<T> GeneratedIds();
    }
    public class IdGenerator<T> : IIdGenerator<T>
    {
        private readonly ISet<T> _GeneratedIds = new HashSet<T>();
        private readonly Func<T, T> _GenerateNewId;
        T LastId = default;
        public IdGenerator(Func<T, T> generateNewId)
        {
            this._GenerateNewId = generateNewId;
        }
        public T GenerateNewId()
        {
            this.LastId = this._GenerateNewId(this.LastId);
            return this.LastId;
        }
        public ISet<T> GeneratedIds() => new HashSet<T>(this._GeneratedIds);
    }
    public static class IdGenerator
    {
        /// <summary>
        /// Represents an id-generator which generates increasing ids beginning with 0.
        /// </summary>
        public static IdGenerator<int> GetDefaultIntIdGenerator() => new IdGenerator<int>((int lastGeneratedId) => lastGeneratedId + 1);
        /// <summary>
        /// Represents an id-generator which generates increasing ids beginning with 0.
        /// </summary>
        public static IdGenerator<long> GetDefaultLongIdGenerator() => new IdGenerator<long>((long lastGeneratedId) => lastGeneratedId + 1);
        /// <summary>
        /// Represents an id-generator which generates increasing ids beginning with 0.
        /// </summary>
        public static IdGenerator<BigInteger> GetDefaultBigIntegerIdGenerator() => new IdGenerator<BigInteger>((BigInteger lastGeneratedId) => lastGeneratedId + 1);
        /// <summary>
        /// Represents an id-generator which generates random guids.
        /// </summary>
        public static IdGenerator<Guid> GetDefaultGuidIdGenerator() => new IdGenerator<Guid>((Guid lastGeneratedId) => Guid.NewGuid());
    }
}