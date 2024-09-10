using System.Data.Common;
using CounterStrikeSharp.API.Core;
using Dapper;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Player;

namespace GenericDB;

public abstract class AbstractDBRankManager(IPlayerManager playerMgr,
  string connectionString, string table = "gang_ranks", bool testing = false)
  : IRankManager {
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


  public void Dispose() {
    Transaction?.Dispose();
    Connection.Dispose();
  }

  public async Task<Dictionary<int, IEnumerable<IGangRank>>> GetAllRanks() {
    var ranks = await Connection.QueryAsync<DBRank>($"SELECT * FROM {table}");
    return ranks.GroupBy(r => r.GangId)
     .ToDictionary(g => g.Key, g => g.AsEnumerable<IGangRank>());
  }

  public async Task<IEnumerable<IGangRank>> GetRanks(int gang) {
    return await Connection.QueryAsync<DBRank>(
      $"SELECT * FROM {table} WHERE GangId = @gang", new { gang }, Transaction);
  }

  public async Task<IGangRank?> GetRank(int gang, int rank) {
    return await Connection.QueryFirstOrDefaultAsync<DBRank>(
      $"SELECT * FROM {table} WHERE GangId = @gang AND `Rank` = @rank",
      new { gang, rank }, Transaction);
  }

  public async Task<bool> AddRank(int gang, IGangRank rank) {
    if (rank.Rank < 0) return false;
    if (await GetRank(gang, rank.Rank) != null) return false;

    var query =
      $"INSERT INTO {table} (GangId, `Rank`, Name, Permissions) VALUES (@GangId, @Rank, @Name, @Perms)";
    return await Connection.ExecuteAsync(query,
      new { GangId = gang, rank.Rank, rank.Name, Perms = rank.Permissions },
      Transaction) == 1;
  }

  public async Task<IGangRank?> CreateRank(int gang, string name, int rank,
    Perm perm) {
    var rankObj = new DBRank {
      GangId = gang, Rank = rank, Name = name, Permissions = perm
    };

    var success = await AddRank(gang, rankObj);
    return success ? rankObj : null;
  }

  public async Task<bool> DeleteRank(int gang, int rank,
    IRankManager.DeleteStrat strat) {
    if (rank <= 0) return false;
    // Check if any players have this rank

    if (strat == IRankManager.DeleteStrat.CANCEL) {
      var prePlayerCheck = await playerMgr.GetMembers(gang);
      if (prePlayerCheck.Any(p => p.GangRank == rank)) return false;
    }

    var lowerRank = await Connection.QueryFirstOrDefaultAsync<DBRank>(
      $"SELECT * FROM {table} WHERE GangId = @GangId AND `Rank` > @Rank ORDER BY `Rank` ASC LIMIT 1",
      new { GangId = gang, rank }, Transaction);

    var members = (await playerMgr.GetMembers(gang))
     .Where(p => p.GangRank == rank)
     .ToList();

    if (strat == IRankManager.DeleteStrat.DEMOTE_FAIL && lowerRank == null
      && members.Count != 0)
      return false;

    foreach (var player in members) {
      player.GangId   = lowerRank?.GangId ?? null;
      player.GangRank = lowerRank?.Rank ?? null;
      await playerMgr.UpdatePlayer(player);
    }

    var query =
      $"DELETE FROM {table} WHERE GangId = @GangId AND `Rank` = @Rank";

    return await Connection.ExecuteAsync(query, new { GangId = gang, rank },
      Transaction) == 1;
  }

  public async Task<bool> DeleteAllRanks(int gang) {
    var query = $"DELETE FROM {table} WHERE GangId = @GangId";
    return await Connection.ExecuteAsync(query, new { gang }, Transaction) > 0;
  }

  public async Task<bool> UpdateRank(int gang, IGangRank rank) {
    switch (rank.Rank) {
      case < 0:
      case > 0 when rank.Permissions.HasFlag(Perm.OWNER):
        return false;
      default: {
        // Update name and permissions
        var query =
          $"UPDATE {table} SET Name = @Name, Permissions = @Perms WHERE GangId = @GangId AND `Rank` = @Rank";
        return await Connection.ExecuteAsync(query,
          new { rank.Name, Perms = rank.Permissions, gang, rank.Rank },
          Transaction) == 1;
      }
    }
  }

  abstract protected DbConnection CreateDbConnection(string connectionString);

  virtual protected string CreateTableQuery(string tableName, bool inTesting) {
    return inTesting ?
      $"CREATE TEMPORARY TABLE IF NOT EXISTS {tableName} (GangId INT NOT NULL, `Rank` INT NOT NULL, Name VARCHAR(255) NOT NULL, Permissions INT NOT NULL, PRIMARY KEY (GangId, `Rank`))" :
      $"CREATE TABLE IF NOT EXISTS {tableName} (GangId INT NOT NULL, `Rank` INT NOT NULL, Name VARCHAR(255) NOT NULL, Permissions INT NOT NULL, PRIMARY KEY (GangId, `Rank`))";
  }
}