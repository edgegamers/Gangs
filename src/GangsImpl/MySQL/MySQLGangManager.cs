using System.Data.Common;
using GangsAPI.Data;
using GangsAPI.Services;
using GangsAPI.Services.Player;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class MySQLGangManager(IServiceProvider provider, IDBConfig config)
  : AbstractDBGangManager(provider, config.ConnectionString,
    config.TablePrefix + "_gangs", config.Testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}