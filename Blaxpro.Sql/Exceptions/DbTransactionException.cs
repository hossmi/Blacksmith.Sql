using System;
using System.Runtime.Serialization;

namespace Blaxpro.Sql.Exceptions
{
    [Serializable]
    public class DbTransactionException : Exception
    {
        public DbTransactionException()
        {
        }

        public DbTransactionException(string message) : base(message)
        {
        }

        public DbTransactionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DbTransactionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}