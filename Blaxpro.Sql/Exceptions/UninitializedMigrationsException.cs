using System;
using System.Runtime.Serialization;

namespace Blaxpro.Sql.Exceptions
{
    public class UninitializedMigrationsException : DbMigrationException
    {
        public UninitializedMigrationsException(string message) : base(message)
        {
        }

        public UninitializedMigrationsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected UninitializedMigrationsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}