using System.Data.Common;
using GangsAPI.Data;
using GangsAPI.Services.Player;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class MySQLGangManager(IPlayerManager playerMgr, IDBConfig config)
  : AbstractDBGangManager(playerMgr, config.ConnectionString,
    config.TablePrefix + "_gangs", config.Testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}