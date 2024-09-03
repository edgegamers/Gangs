using System.Data.Common;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class SQLGangManager(string connectionString,
  string table = "gang_gangs", bool testing = false)
  : AbstractDBGangManager(connectionString, table, testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}