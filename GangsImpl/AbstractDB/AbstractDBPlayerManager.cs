using System.Data.Common;
using CounterStrikeSharp.API.Core;
using Dapper;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Player;
using Mock;

namespace GenericDB;

public abstract class AbstractDBPlayerManager(string connectionString,
  string table = "gang_players", bool testing = false) : IPlayerManager {
  protected DbConnection Connection = null!;
  protected DbTransaction? Transaction;
  public void ClearCache() { }
  public Task Load() { return Task.CompletedTask; }

  public void Start(BasePlugin? plugin, bool hotReload) {
    Connection = CreateDbConnection(connectionString);

    Connection.Open();

    if (testing) Transaction = Connection.BeginTransaction();

    try {
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

  abstract protected DbConnection CreateDbConnection(string connectionString);

  virtual protected string CreateTableQuery(string tableName, bool inTesting) {
    return inTesting ?
      $"CREATE TEMPORARY TABLE IF NOT EXISTS {tableName} (Steam INT NOT NULL AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255), GangId INT, GangRank INT)" :
      $"CREATE TABLE IF NOT EXISTS {tableName} (Steam INT NOT NULL AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255), GangId INT, GangRank INT)";
  }

  public void Dispose() {
    Transaction?.Dispose();
    Connection.Dispose();
  }

  public async Task<IGangPlayer?> GetPlayer(ulong steamId, bool create = true) {
    var query = $"SELECT * FROM {table} WHERE Steam = @steamId";
    return await Connection.QueryFirstOrDefaultAsync<DBPlayer>(query,
      new { steamId }, Transaction);
  }

  public async Task<IGangPlayer> CreatePlayer(ulong steamId,
    string? name = null) {
    var existing = await GetPlayer(steamId, false);
    if (existing != null) return existing;
    var player = new DBPlayer() { Steam = steamId, Name = name };
    var query  = $"INSERT INTO {table} (Steam, Name) VALUES (@Steam, @Name)";
    await Connection.ExecuteAsync(query, player, Transaction);
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
    var query =
      $"UPDATE {table} SET Name = @Name, GangId = @GangId, GangRank = @GangRank WHERE Steam = @Steam";
    return await Connection.ExecuteAsync(query, player, Transaction) == 1;
  }

  public async Task<bool> DeletePlayer(ulong steamId) {
    var query = $"DELETE FROM {table} WHERE Steam = @steamId";
    return await Connection.ExecuteAsync(query, new { steamId }, Transaction)
      == 1;
  }
}