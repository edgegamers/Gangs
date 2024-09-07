using System.Data.Common;
using GangsAPI.Services;
using GangsAPI.Services.Player;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class MySQLGangManager(IPlayerManager playerMgr, string connectionString,
  string table = "gang_gangs", bool testing = false)
  : AbstractDBGangManager(playerMgr, connectionString, table, testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}