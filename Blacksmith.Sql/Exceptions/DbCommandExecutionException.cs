using System;
using System.Runtime.Serialization;

namespace Blacksmith.Sql.Exceptions
{
    [Serializable]
    public class DbCommandExecutionException : Exception
    {
        public DbCommandExecutionException(Exception innerException) : base("Error on command execution.", innerException)
        {
        }

        protected DbCommandExecutionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}