using System.Data.Common;
using GangsAPI.Data;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class MySQLStatManager(IDBConfig config) : AbstractDBStatManager(
  config.ConnectionString, config.TablePrefix + "_stats", config.Testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}