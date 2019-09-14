namespace Blaxpro.Sql
{
    public interface IMigration
    {
        int SourceVersion { get; }
        int TargetVersion { get; }
        void upgrade(ITransaction transaction);
        void downgrade(ITransaction transaction);
    }
}