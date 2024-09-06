using System.Data.Common;
using GangsAPI.Services;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class SQLGangManager(IPlayerManager playerMgr, string connectionString,
  string table = "gang_gangs", bool testing = false)
  : AbstractDBGangManager(playerMgr, connectionString, table, testing) {
  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}