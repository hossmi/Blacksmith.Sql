﻿namespace Blaxpro.Sql.Tests
{
    public class V2AddUserBirthDateColumnMigration : IMigration
    {
        public int SourceVersion { get; }
        public int TargetVersion { get; }

        public void downgrade(ICommandExecutor setter)
        {
            throw new System.NotImplementedException();
        }

        public void upgrade(ICommandExecutor setter)
        {
            throw new System.NotImplementedException();
        }
    }
}