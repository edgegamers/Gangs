﻿using System.Data.Common;
using CounterStrikeSharp.API.Core;
using Dapper;
using GangsAPI;
using GangsAPI.Data.Stat;
using GangsAPI.Services;

namespace GenericDB;

public abstract class GenericDBStatManager(string connectionString,
  string table = "gang_stats", bool testing = false) : IStatManager {
  private readonly HashSet<IStat> stats = [];
  private DbConnection connection = null!;
  private DbTransaction? transaction;

  public void Start(BasePlugin? plugin, bool hotReload) {
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

      LoadStats().GetAwaiter().GetResult();
    } catch (Exception e) {
      transaction?.Rollback();
      throw new InvalidOperationException("Failed initializing the database",
        e);
    }
  }

  protected async Task LoadStats() {
    (await connection.QueryAsync<DBStat>($"SELECT * FROM {table}",
        transaction: transaction)).ToList()
     .ForEach(stat => stats.Add(stat));
  }

  public void Dispose() {
    transaction?.Commit();
    connection.Close();
    connection.Dispose();
  }

  public async Task<IEnumerable<IStat>> GetStats() {
    return await Task.FromResult<IEnumerable<IStat>>(stats);
  }

  public Task<IStat?> GetStat(string id) {
    return Task.FromResult(stats.FirstOrDefault(stat => stat.StatId == id));
  }

  public async Task<IStat?> CreateStat(string id, string name,
    string? description = null) {
    var stat = await GetStat(id);
    if (stat != null) return stat;
    // stat = new DBStat { StatId = id, Name = name, Description = description };
    stat = new DBStat(id, name, description);
    return stat;
  }

  public async Task<bool> RegisterStat(IStat stat) {
    if (stats.Contains(stat)) return false;
    var sqlStat = (DBStat)stat;
    var command = connection.CreateCommand();
    command.Transaction = transaction;
    command.CommandText =
      $"INSERT INTO {table} (StatId, Name, Description) VALUES (@StatId, @Name, @Description)";

    command.Parameters.Add(CreateDbParameter("@StatId", sqlStat.StatId));
    command.Parameters.Add(CreateDbParameter("@Name", sqlStat.Name));

    if (sqlStat.Description != null)
      command.Parameters.Add(CreateDbParameter("@Description",
        sqlStat.Description));
    else
      command.Parameters.Add(CreateDbParameter("@Description", DBNull.Value));

    await command.ExecuteNonQueryAsync();
    return stats.Add(stat);
  }

  public async Task<bool> UnregisterStat(string id) {
    var matches = stats.Where(stat => stat.StatId == id).ToList();
    foreach (var stat in matches) stats.Remove(stat);

    var command = connection.CreateCommand();
    command.Transaction = transaction;
    command.CommandText = $"DELETE FROM {table} WHERE StatId = @StatId";
    command.Parameters.Add(CreateDbParameter("@StatId", id));
    await command.ExecuteNonQueryAsync();

    return await Task.FromResult(matches.Count > 0);
  }

  public abstract DbConnection CreateDbConnection(string connectionString);

  public abstract DbParameter CreateDbParameter(string key, object value);

  public void ClearCache() {
    stats.Clear();
    LoadStats().GetAwaiter().GetResult();
  }
}