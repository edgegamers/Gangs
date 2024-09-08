using System.Data.Common;
using GenericDB;
using Microsoft.Data.Sqlite;

namespace SQLite;

public class SQLitePlayerManager(string connectionString,
  string table = "gang_players", bool testing = false)
  : AbstractDBPlayerManager(connectionString, table, testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new SqliteConnection(connectionString);
  }

  override protected string CreateTableQuery(string tableName, bool inTesting) {
    return inTesting ?
      $"CREATE TEMPORARY TABLE IF NOT EXISTS {tableName} (Steam INTEGER PRIMARY KEY, Name VARCHAR(255), GangId INTEGER, GangRank INTEGER)" :
      $"CREATE TABLE IF NOT EXISTS {tableName} (Steam INTEGER PRIMARY KEY, Name VARCHAR(255), GangId INTEGER, GangRank INTEGER)";
  }
}