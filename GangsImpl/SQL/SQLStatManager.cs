using System.Data.Common;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class SQLStatManager(string connectionString,
  string table = "gang_stats", bool testing = false)
  : AbstractDBStatManager(connectionString, table, testing) {
  public override DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}