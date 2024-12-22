using System;

namespace GRYLibrary.Core.Exceptions
{
    /// <summary>
    /// Represents an exception which indicates that the algorithm where this exception will be thrown is supposed to be aborted.
    /// </summary>
    public class AbortException : Exception
    {
        public Exception InnerException { get; private set; }
        public AbortException(Exception innerException) : base()
        {
            this.InnerException = innerException;
        }
        public AbortException(Exception innerException, string message) : base(message)
        {
            this.InnerException = innerException;
        }

        public override string ToString()
        {
            string result = $"Aborted";
            if (this.Message != null)
            {
                result = $"{result} ({this.Message})";
            }
            result = $"{result} ({this.InnerException.ToString()})";
            return result;
        }
    }
}
