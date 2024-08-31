using System.Data.Common;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class SqlStatStatManager(string connectionString,
  string table = "gang_stats", bool testing = false)
  : GenericDBStatManager(connectionString, table, testing) {
  public override DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }

  public override DbParameter CreateDbParameter(string key, object value) {
    return new MySqlParameter(key, value);
  }
}