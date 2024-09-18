﻿using System.Data.Common;
using CounterStrikeSharp.API.Core;
using Dapper;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Player;

namespace GenericDB;

public abstract class AbstractDBPlayerManager(string connectionString,
  string table = "gang_players", bool testing = false) : IPlayerManager {
  protected DbConnection Connection = null!;
  protected DbTransaction? Transaction;

  private Dictionary<ulong, IGangPlayer> cache = new();

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

  public void Dispose() {
    Transaction?.Dispose();
    Connection.Dispose();
  }

  public async Task<IGangPlayer?> GetPlayer(ulong steamId, bool create = true) {
    if (cache.TryGetValue(steamId, out var player)) return player;
    var query = $"SELECT * FROM {table} WHERE Steam = @steamId";
    var result = await Connection.QueryFirstOrDefaultAsync<DBPlayer>(query,
      new { steamId }, Transaction);
    if (result != null) cache[steamId] = result;
    if (result != null || !create) return result;
    return await CreatePlayer(steamId);
  }

  public async Task<IGangPlayer> CreatePlayer(ulong steamId,
    string? name = null) {
    var existing = await GetPlayer(steamId, false);
    if (existing != null) return existing;
    var player = new DBPlayer { Steam = steamId, Name = name };
    var query  = $"INSERT INTO {table} (Steam, Name) VALUES (@Steam, @Name)";
    await Connection.ExecuteAsync(query, player, Transaction);
    cache[steamId] = player;
    return player;
  }

  public async Task<IEnumerable<IGangPlayer>> GetAllPlayers() {
    var query = $"SELECT * FROM {table}";
    return await Connection.QueryAsync<DBPlayer>(query,
      transaction: Transaction);
  }

  public async Task<IEnumerable<IGangPlayer>> GetMembers(int gangId) {
    var query = $"SELECT * FROM {table} WHERE GangId = @gangId";
    return await Connection.QueryAsync<DBPlayer>(query, new { gangId },
      Transaction);
  }

  public async Task<bool> UpdatePlayer(IGangPlayer player) {
    if (player.GangId == null != (player.GangRank == null))
      throw new InvalidOperationException(
        "Player must have both GangId and GangRank set or neither set");

    var query =
      $"UPDATE {table} SET Name = @Name, GangId = @GangId, GangRank = @GangRank WHERE Steam = @Steam";
    cache[player.Steam] = player;
    return await Connection.ExecuteAsync(query, player, Transaction) == 1;
  }

  public async Task<bool> DeletePlayer(ulong steamId) {
    var query = $"DELETE FROM {table} WHERE Steam = @steamId";
    cache.Remove(steamId);
    return await Connection.ExecuteAsync(query, new { steamId }, Transaction)
      == 1;
  }

  abstract protected DbConnection CreateDbConnection(string connectionString);

  virtual protected string CreateTableQuery(string tableName, bool inTesting) {
    return inTesting ?
      $"CREATE TEMPORARY TABLE IF NOT EXISTS {tableName} (Steam BIGINT NOT NULL PRIMARY KEY, Name VARCHAR(255), GangId INT, GangRank INT)" :
      $"CREATE TABLE IF NOT EXISTS {tableName} (Steam BIGINT NOT NULL PRIMARY KEY, Name VARCHAR(255), GangId INT, GangRank INT)";
  }
}