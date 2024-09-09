using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;

namespace Mock;

public class MockRankManager : IRankManager {
  protected readonly Dictionary<int, IEnumerable<IGangRank>> Ranks = new();

  public Task<Dictionary<int, IEnumerable<IGangRank>>> GetAllRanks() {
    return Task.FromResult(Ranks);
  }

  public Task<IEnumerable<IGangRank>> GetRanks(int gang) {
    return Task.FromResult(Ranks.GetValueOrDefault(gang) ?? []);
  }

  public Task<IGangRank?> GetRank(int gang, int rank) {
    return Task.FromResult(Ranks.GetValueOrDefault(gang)
    ?.FirstOrDefault(r => r.Rank == rank));
  }

  public async Task<bool> AddRank(int gang, IGangRank rank) {
    if ((await GetRanks(gang)).Any(r => r.Rank == rank.Rank)) return false;
    if (!Ranks.ContainsKey(gang)) Ranks.Add(gang, new List<IGangRank>());
    ((List<IGangRank>)Ranks[gang]).Add(rank);
    return true;
  }

  public async Task<IGangRank?> CreateRank(int gang, string name, int rank,
    IGangRank.Permissions permissions) {
    if (rank < 0) return null;
    var newRank = new MockRank(name, rank, permissions);
    var added   = await AddRank(gang, newRank);
    return added ? newRank : null;
  }

  public Task<bool> DeleteRank(int gang, int rank) {
    if (rank <= 0) return Task.FromResult(false);
    if (!Ranks.TryGetValue(gang, out var gangRanks))
      return Task.FromResult(false);
    gangRanks = gangRanks.ToList();
    if (gangRanks.All(r => r.Rank != rank)) return Task.FromResult(false);
    Ranks[gang] = gangRanks.Where(r => r.Rank != rank);
    return Task.FromResult(true);
  }
}