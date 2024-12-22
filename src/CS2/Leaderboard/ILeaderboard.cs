using GangsAPI;

namespace Leaderboard;

public interface ILeaderboard : IPluginBehavior {
  Task<IEnumerable<(int, double)>> GetTopGangs(int limit = 10, int offset = 0);
  Task<int?> GetPosition(int gang);
}