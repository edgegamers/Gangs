using System.Data.Common;
using Dapper;
using GenericDB;
using Microsoft.Data.Sqlite;

namespace SQLite;

public class SQLiteGangManager(IServiceProvider provider,
  string connectionString, string table = "gang_gangs", bool testing = false)
  : AbstractDBGangManager(provider, connectionString, table, testing) {
  override protected string CreateTableQuery(string tableName, bool inTesting) {
    return inTesting ?
      $"CREATE TEMPORARY TABLE IF NOT EXISTS {tableName} (GangId INTEGER PRIMARY KEY, Name VARCHAR(255) NOT NULL)" :
      $"CREATE TABLE IF NOT EXISTS {tableName} (GangId INTEGER PRIMARY KEY, Name VARCHAR(255) NOT NULL)";
  }

  override protected DbConnection CreateDbConnection(string connectionString) {
    return new SqliteConnection(connectionString);
  }
}