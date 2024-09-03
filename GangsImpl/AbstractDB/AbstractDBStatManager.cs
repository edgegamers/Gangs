﻿using System.Data.Common;
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

      // TODO: For some reason GitHub CI fails here
      // LoadStats().GetAwaiter().GetResult();
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
    if (cachedStats.Contains(stat)) return false;
    var sqlStat = (DBStat)stat;
    // var command = connection.CreateCommand();
    await connection.ExecuteAsync(
      $"INSERT INTO {table} (StatId, Name, Description) VALUES (@StatId, @Name, @Description)",
      new { sqlStat.StatId, sqlStat.Name, sqlStat.Description }, transaction);
    return cachedStats.Add(stat);
  }

  public override async Task<bool> UnregisterStat(string id) {
    var matches = cachedStats.Where(stat => stat.StatId == id).ToList();
    foreach (var stat in matches) cachedStats.Remove(stat);

    await connection.ExecuteAsync($"DELETE FROM {table} WHERE StatId = @StatId",
      new { StatId = id }, transaction);

    return await Task.FromResult(matches.Count > 0);
  }

  public override async Task Load() {
    var query = $"SELECT * FROM {table}";
    var result = await connection.QueryAsync<DBStat>(query,
      transaction: transaction);
    foreach (var stat in result) cachedStats.Add(stat);
  }

  public abstract DbConnection CreateDbConnection(string connectionString);

  // public abstract DbParameter CreateDbParameter(string key, object value);
}