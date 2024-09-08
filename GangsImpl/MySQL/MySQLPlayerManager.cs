using System.Data.Common;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class MySQLPlayerManager(string connectionString,
  string table = "gang_players", bool testing = false)
  : AbstractDBPlayerManager(connectionString, table, testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}