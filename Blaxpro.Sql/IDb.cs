namespace Blaxpro.Sql
{
    public interface IDb
    {
        ITransaction transact();
    }
}