using System.Data.Common;
using CounterStrikeSharp.API.Core;
using Dapper;
using GangsAPI.Data.Stat;
using Mock;

namespace GenericDB;

public abstract class AbstractDBStatManager(string connectionString,
  string table = "gang_stats", bool testing = false) : MockStatManager {
  private DbConnection connection = null!;
  private DbTransaction? transaction;

  public override void Start(BasePlugin? plugin, bool hotReload) {
    connection = CreateDbConnection(connectionString);

    connection.Open();

    if (testing) transaction = connection.BeginTransaction();

    try {
      var command = connection.CreateCommand();

      command.Transaction = transaction;
      command.CommandText = testing ?
        $"CREATE TEMPORARY TABLE IF NOT EXISTS {table} (StatId VARCHAR(255) NOT NULL PRIMARY KEY, Name VARCHAR(255) NOT NULL, Description TEXT)" :
        $"CREATE TABLE IF NOT EXISTS {table} (StatId VARCHAR(255) NOT NULL PRIMARY KEY, Name VARCHAR(255) NOT NULL, Description TEXT)";

      command.ExecuteNonQuery();
    } catch (Exception e) {
      transaction?.Rollback();
      throw new InvalidOperationException("Failed initializing the database",
        e);
    }
  }

  public void Dispose() {
    transaction?.Commit();
    connection.Close();
    connection.Dispose();
  }

  public override async Task<IStat?> CreateStat(string id, string name,
    string? description = null) {
    var stat = await GetStat(id);
    if (stat != null) return stat;
    stat = new DBStat(id, name, description);
    return stat;
  }

  public override async Task<bool> RegisterStat(IStat stat) {
    if (CachedStats.Contains(stat)) return false;
    await connection.ExecuteAsync(
      $"INSERT INTO {table} (StatId, Name, Description) VALUES (@StatId, @Name, @Description)",
      new { stat.StatId, stat.Name, stat.Description }, transaction);
    return CachedStats.Add(stat);
  }

  public override async Task<bool> UnregisterStat(string id) {
    var matches = CachedStats.Where(stat => stat.StatId == id).ToList();
    foreach (var stat in matches) CachedStats.Remove(stat);

    await connection.ExecuteAsync($"DELETE FROM {table} WHERE StatId = @StatId",
      new { StatId = id }, transaction);

    return await Task.FromResult(matches.Count > 0);
  }

  public override async Task Load() {
    var query = $"SELECT * FROM {table}";
    var result = await connection.QueryAsync<DBStat>(query,
      transaction: transaction);
    foreach (var stat in result) CachedStats.Add(stat);
  }

  abstract protected DbConnection CreateDbConnection(string connectionString);
}