using SQLite;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Example
{
  public sealed class Database
  {
    public static Database Instance
    {
      get { return instance; }
    }

    static Database() { }
    private Database() { }

    private static Database instance = new Database();
    private static SQLiteAsyncConnection sqliteConnection;

    public bool IsConnected()
    {
      if (sqliteConnection != null)
      {
        return true;
      }
      else
      {
        return false;
      }
    }

    public async Task Connect(string databaseFilePath)
    {
      TryDisconnect();

      sqliteConnection = new SQLiteAsyncConnection(databaseFilePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);

      await sqliteConnection.EnableLoadExtensionAsync(false);
    }

    private void TryDisconnect()
    {
      if (sqliteConnection != null)
      {
        sqliteConnection.CloseAsync();
        
        sqliteConnection = null;
      }
    }

    public async Task<string> ScalarStringQuery(string query)
    {
      return await sqliteConnection.ExecuteScalarAsync<string>(query);
    }

    public async Task<int> ScalarIntQuery(string query)
    {
      return await sqliteConnection.ExecuteScalarAsync<int>(query);
    }

    public async Task<List<T>> QueryCollection<T>(string query) where T : new()
    {
      return await sqliteConnection.QueryAsync<T>(query);
    }

    ~Database()
    {
      TryDisconnect();
    }
  }
}