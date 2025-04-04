﻿using System.Data.Common;
using CounterStrikeSharp.API.Core;
using Dapper;
using GangsAPI.Data.Gang;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;

namespace GenericDB;

public abstract class AbstractDBGangManager(IServiceProvider provider,
  string connectionString, string table = "gang_gangs", bool testing = false)
  : IGangManager {
  private readonly IDictionary<int, IGang> cache =
    new Cache<int, IGang>(TimeSpan.FromMinutes(10));

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly SemaphoreSlim semaphore = new(1, 1);

  protected DbConnection Connection = null!;
  protected DbTransaction? Transaction;

  public void Start(BasePlugin? plugin, bool hotReload) {
    Connection = CreateDbConnection(connectionString);

    try {
      Connection.Open();

      if (testing) Transaction = Connection.BeginTransaction();

      var command = Connection.CreateCommand();

      command.Transaction = Transaction;
      command.CommandText = CreateTableQuery(table, testing);

      command.ExecuteNonQuery();
    } catch (Exception e) {
      Transaction?.Rollback();
      throw new InvalidOperationException("Failed initializing the database",
        e);
    }
  }

  public async Task<IEnumerable<IGang>> GetGangs() {
    var query = $"SELECT * FROM {table}";
    return await Connection.QueryAsync<DBGang>(query, transaction: Transaction);
  }

  public async Task<IGang?> GetGang(int id) {
    if (cache.TryGetValue(id, out var cached)) return cached;
    var query = $"SELECT * FROM {table} WHERE GangId = @id";

    try {
      await semaphore.WaitAsync(TimeSpan.FromSeconds(1));
      var result =
        await Connection.QueryFirstOrDefaultAsync<DBGang>(query, new { id },
          Transaction);

      if (result == null) return null;
      cache[id] = result;
      return result;
    } finally { semaphore.Release(); }
  }

  public async Task<IGang?> GetGang(ulong steam) {
    var player = await players.GetPlayer(steam);
    if (player?.GangId == null) return null;
    return await GetGang(player.GangId.Value);
  }

  public async Task<bool> UpdateGang(IGang gang) {
    var query = $"UPDATE {table} SET Name = @Name WHERE GangId = @GangId";
    cache[gang.GangId] = gang;

    try {
      await semaphore.WaitAsync(TimeSpan.FromSeconds(1));
      return await Connection.ExecuteAsync(query,
        new { gang.Name, gang.GangId }, Transaction) == 1;
    } finally { semaphore.Release(); }
  }

  public async Task<bool> DeleteGang(int id) {
    var members = await players.GetMembers(id);
    foreach (var member in members) {
      member.GangId   = null;
      member.GangRank = null;
      await players.UpdatePlayer(member);
    }

    await ranks.DeleteAllRanks(id);

    cache.Remove(id);
    var query = $"DELETE FROM {table} WHERE GangId = @id";

    try {
      await semaphore.WaitAsync(TimeSpan.FromSeconds(1));
      return await Connection.ExecuteAsync(query, new { id }, Transaction) > 0;
    } finally { semaphore.Release(); }
  }

  public async Task<IGang?> CreateGang(string name, ulong owner) {
    var player = await players.GetPlayer(owner);
    if (player == null) {
      Console.WriteLine(
        $"Failed to create gang {name}: player {owner} not found");
      return null;
    }

    if (player.GangId != null)
      throw new InvalidOperationException(
        $"Attempted to create a gang for {owner} who is already in gang {player.GangId}");

    var query = $"INSERT INTO {table} (Name) VALUES (@name)";
    try {
      await semaphore.WaitAsync(TimeSpan.FromSeconds(1));
      var result =
        await Connection.ExecuteAsync(query, new { name }, Transaction);
      if (result == 0) {
        Console.WriteLine($"Failed to create gang {name}: database returned 0");
        return null;
      }
    } finally { semaphore.Release(); }

    var id = await GetLastId();

    await ranks.AssignDefaultRanks(id);

    player.GangId   = id;
    player.GangRank = 0;
    await players.UpdatePlayer(player);
    var gang = new DBGang(id, name);
    return gang.Clone() as IGang;
  }

  public void Dispose() {
    Transaction?.Dispose();
    Connection.Dispose();
  }

  abstract protected DbConnection CreateDbConnection(string connectionString);

  virtual protected string CreateTableQuery(string tableName, bool inTesting) {
    return inTesting ?
      $"CREATE TEMPORARY TABLE IF NOT EXISTS {tableName} (GangId INT NOT NULL AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255) NOT NULL)" :
      $"CREATE TABLE IF NOT EXISTS {tableName} (GangId INT NOT NULL AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255) NOT NULL)";
  }

  virtual protected async Task<int> GetLastId() {
    return await Connection.ExecuteScalarAsync<int>(
      $"SELECT MAX(GangId) FROM {table}", transaction: Transaction);
  }
}