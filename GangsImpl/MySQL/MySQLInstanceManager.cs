using System.Data.Common;
using GangsAPI;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using GenericDB;
using MySqlConnector;

namespace SQLImpl;

public class SQLGangInstanceManager(string connectionString,
  string table_prefix, bool testing = false)
  : AbstractInstanceManager<int>(connectionString, table_prefix, testing),
    IGangStatManager {
  override protected string PrimaryKey => "GangId";

  public Task<(bool, TV?)> GetForGang<TV>(int key, string statId)
    => Get<TV>(key, statId);

  public Task<bool> SetForGang<TV>(int gangId, string statId, TV value)
    => Set(gangId, statId, value);

  public Task<bool> RemoveFromGang(int gangId, string statId)
    => Remove(gangId, statId);

  override protected DbConnection CreateDbConnection(string connectionString)
    => new MySqlConnection(connectionString);
}

public class SQLPlayerInstanceManager(string connectionString,
  string table_prefix, bool testing = false)
  : AbstractInstanceManager<ulong>(connectionString, table_prefix, testing),
    IPlayerStatManager {
  override protected string PrimaryKey => "Steam";

  public Task<(bool, TV?)> GetForPlayer<TV>(ulong steam, string statId) {
    return Get<TV>(steam, statId);
  }

  public Task<bool> SetForPlayer<TV>(ulong steam, string statId, TV value) {
    return Set(steam, statId, value);
  }

  public Task<bool> RemoveFromPlayer(ulong steam, string statId) {
    return Remove(steam, statId);
  }

  override protected DbConnection CreateDbConnection(string connectionString)
    => new MySqlConnection(connectionString);
}