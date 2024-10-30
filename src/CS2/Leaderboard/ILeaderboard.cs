namespace Leaderboard;

public interface ILeaderboard {
  Task<IEnumerable<(int, double)>> GetTopGangs(int limit = 10, int offset = 0);
}