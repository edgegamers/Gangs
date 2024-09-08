using System.Data.Common;
using Dapper;
using GangsAPI.Services.Player;
using GenericDB;
using Microsoft.Data.Sqlite;

namespace SQLite;

public class SQLiteGangManager(IPlayerManager playerMgr,
  string connectionString, string table = "gang_gangs", bool testing = false)
  : AbstractDBGangManager(playerMgr, connectionString, table, testing) {
  override protected string CreateTableQuery(string tableName, bool inTesting) {
    return inTesting ?
      $"CREATE TEMPORARY TABLE IF NOT EXISTS {tableName} (GangId INTEGER PRIMARY KEY, Name VARCHAR(255) NOT NULL)" :
      $"CREATE TABLE IF NOT EXISTS {tableName} (GangId INTEGER PRIMARY KEY, Name VARCHAR(255) NOT NULL)";
  }

  override protected Task<int> GetLastId() {
    return Connection.ExecuteScalarAsync<int>("SELECT last_insert_rowid()",
      transaction: Transaction);
  }

  override protected DbConnection CreateDbConnection(string connectionString) {
    return new SqliteConnection(connectionString);
  }
}