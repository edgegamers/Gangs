using System.Data.Common;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class SqlStatStatManager(string connectionString,
  string table = "gang_stats") : GenericDBStatManager(connectionString, table) {
  public override DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }

  public override DbParameter CreateDbParameter(string key, object value) {
    return new MySqlParameter(key, value);
  }
}