﻿using System;
using System.Runtime.Serialization;

namespace Blacksmith.Sql.Exceptions
{
    public class DbMigrationException : Exception
    {
        public DbMigrationException(string message) : base(message)
        {
        }

        public DbMigrationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DbMigrationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}