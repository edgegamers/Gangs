using System.Data.Common;
using System.Reflection;
using GangsAPI.Services.Gang;
using GenericDB;
using Microsoft.Data.Sqlite;

namespace SQLite;

public class SQLiteGangInstanceManager(string connectionString,
  string tablePrefix, bool testing = false)
  : AbstractInstanceManager<int>(connectionString, tablePrefix, testing),
    IGangStatManager {
  private readonly string myTablePrefix = tablePrefix;
  override protected string PrimaryKey => "GangId";

  override protected DbConnection CreateDbConnection(string connectionString) {
    return new SqliteConnection(connectionString);
  }

  override protected string GenerateInsertQuery<TV>(string statId,
    IList<PropertyInfo> properties) {
    var columns = GetFieldNames<TV>();
    var values  = GetFieldNames<TV>("@");

    if (typeof(TV).IsPrimitive) {
      columns = statId;
      values  = $"@{statId}";
    }

    var onDuplicate = string.Join(", ",
      properties.Select(f => $"{f.Name} = @{f.Name}"));
    if (typeof(TV).IsPrimitive) onDuplicate = $"{statId} = @{statId}";
    return
      $"INSERT INTO {myTablePrefix}_{statId} ({PrimaryKey}, {columns}) VALUES (@{PrimaryKey}, {values}) ON CONFLICT({PrimaryKey}) DO UPDATE SET {onDuplicate}";
  }

  public Task<(bool, TV?)> GetForGang<TV>(int key, string statId)
    => Get<TV>(key, statId);

  public Task<bool> SetForGang<TV>(int gangId, string statId, TV value)
    => Set(gangId, statId, value);

  public Task<bool> RemoveFromGang(int gangId, string statId)
    => Remove(gangId, statId);
}