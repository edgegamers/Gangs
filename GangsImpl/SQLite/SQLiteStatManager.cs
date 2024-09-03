using System.Data.Common;
using GenericDB;
using Microsoft.Data.Sqlite;

namespace SQLite;

public class SQLiteStatManager(string connectionString,
  string table = "gang_stats", bool testing = false)
  : AbstractDBStatManager(connectionString, table, testing) {
  public override DbConnection CreateDbConnection(string connectionString) {
    return new SqliteConnection(connectionString);
  }
}