namespace Example
{
  public static class DatabaseQueries
  {
    // Randomly generated by database generation tool.
    public static string TableName = "why_PM_if";

    // https://blog.xojo.com/2019/02/11/tip-sqlite-in-ram-to-improve-speed/
    public static string SpeedupReadsQuery = "PRAGMA LOCKING_MODE = Exclusive;";
    public static string CreateInMemoryDbQuery = "attach database ':memory:' as 'RAMDB';";
    public static string CopyCollectionToInMemoryDbQuery = $"create table RAMDB.{TableName} as select * from main.{TableName};";
    public static string CollectionCountQuery = $"select count(distinct(id)) from RAMDB.{TableName};";
    public static string CollectionQuery = $"SELECT id FROM RAMDB.{TableName};";
  }
}