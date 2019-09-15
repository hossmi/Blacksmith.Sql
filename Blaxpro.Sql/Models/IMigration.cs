namespace Blaxpro.Sql
{
    public interface IMigration
    {
        int SourceVersion { get; }
        int TargetVersion { get; }
        void upgrade(ICommandExecutor setter);
        void downgrade(ICommandExecutor setter);
    }
}