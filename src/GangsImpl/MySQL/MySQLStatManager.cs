using System.Data.Common;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class MySQLStatManager(string connectionString,
  string table = "gang_stats", bool testing = false)
  : AbstractDBStatManager(connectionString, table, testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}