using System;
using System.Runtime.Serialization;

namespace Blaxpro.Sql.Exceptions
{
    [Serializable]
    public class AlreadyInitializedMigrationsException : Exception
    {
        public AlreadyInitializedMigrationsException(string message) : base(message)
        {
        }

        public AlreadyInitializedMigrationsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AlreadyInitializedMigrationsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}