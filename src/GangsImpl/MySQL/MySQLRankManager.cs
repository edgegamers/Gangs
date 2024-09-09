using System.Data.Common;
using GangsAPI.Data;
using GangsAPI.Services.Player;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class MySQLRankManager(IPlayerManager playerMgr, IDBConfig config)
  : AbstractDBRankManager(playerMgr, config.ConnectionString,
    config.TablePrefix, config.Testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}