using System.Data.Common;
using GangsAPI.Data;
using GangsAPI.Services;
using GangsAPI.Services.Player;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class MySQLGangManager(IPlayerManager players, IRankManager ranks,
  IDBConfig config) : AbstractDBGangManager(players, ranks,
  config.ConnectionString, config.TablePrefix + "_gangs", config.Testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}