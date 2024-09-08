using System.Data.Common;
using GangsAPI.Data;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class MySQLPlayerManager(IDBConfig config) : AbstractDBPlayerManager(
  config.ConnectionString, config.TablePrefix + "_players", config.Testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}