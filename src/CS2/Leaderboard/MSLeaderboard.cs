using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Dapper;

namespace Leaderboard;

public class MSLeaderboard(IServiceProvider provider, IDBConfig config)
  : ILeaderboard {
  public void Start(BasePlugin? plugin, bool hotReload) {
    if (plugin == null) return;
    var cmd       = provider.GetRequiredService<ICommandManager>();
    var lbCommand = new LeaderboardCommand(this, provider);
    cmd.RegisterCommand(lbCommand);
    plugin.RegisterAllAttributes(lbCommand);
  }

  public async Task<IEnumerable<(int, double)>> GetTopGangs(int limit = 10,
    int offset = 0) {
    await using var connection = new MySqlConnection(config.ConnectionString);
    await connection.OpenAsync();

    var cmd = connection.CreateCommand();
    cmd.CommandText =
      $"SELECT GangId, Score FROM {config.TablePrefix}_leaderboard ORDER BY Score DESC LIMIT @limit OFFSET @offset";

    cmd.Parameters.Add(new MySqlParameter("@limit", limit));
    cmd.Parameters.Add(new MySqlParameter("@offset", offset));

    await using var reader = await cmd.ExecuteReaderAsync();
    var             result = new List<(int, double)>();

    while (await reader.ReadAsync())
      result.Add((reader.GetInt32(0), reader.GetDouble(1)));

    return result;
  }

  public async Task<int?> GetPosition(int gang) {
    await using var connection = new MySqlConnection(config.ConnectionString);
    await connection.OpenAsync();

    var cmd = connection.CreateCommand();
    cmd.CommandText =
      $"SELECT Position FROM {config.TablePrefix}_leaderboard WHERE GangId = @gang LIMIT 1";

    cmd.Parameters.Add(new MySqlParameter("@gang", gang));

    return await connection.QueryFirstOrDefaultAsync<int?>(cmd.CommandText);
  }

  public async Task<int?> GetELO(ulong steam) {
    await using var connection = new MySqlConnection(config.ConnectionString);
    await connection.OpenAsync();

    var cmd = connection.CreateCommand();
    cmd.CommandText =
      $"SELECT ELO FROM {config.TablePrefix}_player_leaderboard WHERE Steam = @steam LIMIT 1";

    cmd.Parameters.Add(new MySqlParameter("@steam", steam));

    return await connection.QueryFirstOrDefaultAsync<int?>(cmd.CommandText);
  }

  public async Task<IEnumerable<ILeaderboard.PlayerRank>> GetTopPlayers(
    int limit = 10, int offset = 0) {
    await using var connection = new MySqlConnection(config.ConnectionString);
    await connection.OpenAsync();

    var cmd = connection.CreateCommand();
    cmd.CommandText =
      $"SELECT Steam, Name, Score, Position, Percentile, ELO FROM {config.TablePrefix}_player_leaderboard ORDER BY Score DESC LIMIT @limit OFFSET @offset";

    cmd.Parameters.Add(new MySqlParameter("@limit", limit));
    cmd.Parameters.Add(new MySqlParameter("@offset", offset));

    return await connection
     .QueryAsync<ILeaderboard.PlayerRank>(cmd.CommandText);
  }

  public async Task<ILeaderboard.PlayerRank?> GetPlayerRank(ulong steam) {
    await using var connection = new MySqlConnection(config.ConnectionString);
    await connection.OpenAsync();

    var cmd = connection.CreateCommand();
    cmd.CommandText =
      $"SELECT Steam, Name, Score, Position, Percentile, ELO FROM {config.TablePrefix}_player_leaderboard WHERE Steam = @steam LIMIT 1";

    cmd.Parameters.Add(new MySqlParameter("@steam", steam));

    return connection
     .Query<ILeaderboard.PlayerRank>(cmd.CommandText, new { steam })
     .FirstOrDefault();
  }
}