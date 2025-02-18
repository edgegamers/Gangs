using System.Data.Common;
using GangsAPI.Data;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class MySQLGangInstanceManager(IDBConfig config)
  : AbstractInstanceManager<int>(config.ConnectionString,
    config.TablePrefix + "_gang_stats", config.Testing), IGangStatManager {
  override protected string PrimaryKey => "GangId";

  public Task<TV?> GetForGang<TV>(int key, string statId) {
    return Get<TV>(key, statId);
  }

  public Task<bool> SetForGang<TV>(int gangId, string statId, TV value) {
    return Set(gangId, statId, value);
  }

  public Task<bool> RemoveFromGang(int gangId, string statId) {
    return Remove(gangId, statId);
  }

  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}

public class MySQLPlayerInstanceManager(IDBConfig config)
  : AbstractInstanceManager<ulong>(config.ConnectionString,
    config.TablePrefix + "_player_stats", config.Testing), IPlayerStatManager {
  override protected string PrimaryKey => "Steam";

  public Task<TV?> GetForPlayer<TV>(ulong steam, string statId) {
    return Get<TV>(steam, statId);
  }

  public Task<bool> SetForPlayer<TV>(ulong steam, string statId, TV value) {
    return Set(steam, statId, value);
  }

  public Task<bool> RemoveFromPlayer(ulong steam, string statId) {
    return Remove(steam, statId);
  }

  override protected DbConnection CreateDbConnection(string connectionString) {
    return new MySqlConnection(connectionString);
  }
}