namespace GangsAPI.Services;

public interface ILeaderboard : IPluginBehavior {
  struct PlayerRank {
    public ulong Steam;
    public string Name;
    public float Score;
    public int Position;
    public float Percentile;
    public float ELO;
  }

  Task<IEnumerable<(int, double)>> GetTopGangs(int limit = 10, int offset = 0);
  Task<int?> GetPosition(int gang);
  Task<int?> GetELO(ulong steam);
  Task<IEnumerable<PlayerRank>> GetTopPlayers(int limit = 10, int offset = 0);
  Task<PlayerRank?> GetPlayerRank(ulong steam);
}