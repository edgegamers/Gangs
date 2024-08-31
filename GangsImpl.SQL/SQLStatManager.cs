using System.Data;
using Dapper;
using GangsAPI.Data.Stat;
using GangsAPI.Services;
using MySqlConnector;

namespace GangsImpl.SQL;

public class SQLStatManager(string connectionString,
  string table = "gang_stats") : IStatManager {
  private readonly HashSet<IStat> stats = [];
  private MySqlConnection connection = null!;

  public void Start() {
    connection = new MySqlConnection(connectionString);

    connection.Open();
    var command = connection.CreateCommand();
    command.CommandText =
      $"CREATE TABLE IF NOT EXISTS {table} (StatId VARCHAR(255) PRIMARY KEY, Name VARCHAR(255), Description TEXT)";
    command.ExecuteNonQuery();

    connection.Query<SQLStat>($"SELECT * FROM {table}")
     .ToList()
     .ForEach(stat => stats.Add(stat));
  }

  public void Dispose() {
    var command = connection.CreateCommand();
    command.CommandText = $"DROP TABLE IF EXISTS {table}";
    command.ExecuteNonQuery();
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
    stat = new SQLStat { StatId = id, Name = name, Description = description };
    return stat;
  }

  public async Task<bool> RegisterStat(IStat stat) {
    var sqlStat = (SQLStat)stat;
    var command = connection.CreateCommand();
    command.CommandText =
      $"INSERT INTO {table} (StatId, Name, Description) VALUES (@StatId, @Name, @Description)";
    command.Parameters.AddWithValue("@StatId", sqlStat.StatId);
    command.Parameters.AddWithValue("@Name", sqlStat.Name);
    command.Parameters.AddWithValue("@Description", sqlStat.Description);
    await command.ExecuteNonQueryAsync();
    return stats.Add(stat);
  }

  public async Task<bool> UnregisterStat(string id) {
    var matches = stats.Where(stat => stat.StatId == id).ToList();
    foreach (var stat in matches) stats.Remove(stat);

    var command = connection.CreateCommand();
    command.CommandText = $"DELETE FROM {table} WHERE StatId = @StatId";
    command.Parameters.AddWithValue("@StatId", id);
    await command.ExecuteNonQueryAsync();

    return await Task.FromResult(matches.Count > 0);
  }
}