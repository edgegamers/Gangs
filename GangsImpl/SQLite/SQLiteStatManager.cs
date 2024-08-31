using System.Data.Common;
using GenericDB;
using Microsoft.Data.Sqlite;

namespace GangsImpl.SQLLite;

public class SQLiteStatManager(string connectionString,
  string table = "gang_stats", bool testing = false)
  : GenericDBStatManager(connectionString, table, testing) {
  public override DbConnection CreateDbConnection(string connectionString) {
    return new SqliteConnection(connectionString);
  }

  public override DbParameter CreateDbParameter(string key, object value) {
    return new SqliteParameter(key, value);
  }
}