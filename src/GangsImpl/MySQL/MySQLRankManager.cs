using System.Data.Common;
using GangsAPI.Data;
using GangsAPI.Services.Player;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class MySQLRankManager(IPlayerManager players, IDBConfig config)
  : AbstractDBRankManager(players, config.ConnectionString,
    config.TablePrefix + "_ranks", config.Testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}