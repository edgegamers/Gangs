using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Player;

namespace Mock;

public class MockRankManager(IPlayerManager players) : IRankManager {
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
    Perm permissions) {
    if (rank < 0) return null;
    var newRank = new MockRank(name, rank, permissions);
    var added   = await AddRank(gang, newRank);
    return added ? newRank : null;
  }

  // public Task<bool> DeleteRank(int gang, int rank, IRankManager.DeleteStrat strat) { throw new NotImplementedException(); }

  public async Task<bool> DeleteRank(int gang, int rank,
    IRankManager.DeleteStrat strat) {
    if (rank <= 0) return false;
    if (!Ranks.TryGetValue(gang, out var gangRanks)) return false;

    if (strat == IRankManager.DeleteStrat.CANCEL) {
      var playersInRank = await players.GetMembers(gang);
      if (playersInRank.Any(p => p.GangRank == rank)) return false;
    }

    gangRanks = gangRanks.ToList();

    var sortedRanks = gangRanks.ToList();

    // Sort from highest ranking to lowest
    sortedRanks.Sort();

    if (gangRanks.All(r => r.Rank != rank)) return false;

    var lowerRank = sortedRanks.FirstOrDefault(r => r.Rank > rank);

    var members = (await players.GetMembers(gang))
     .Where(p => p.GangRank == rank)
     .ToList();

    if (members == null) return false;

    if (strat == IRankManager.DeleteStrat.DEMOTE_FAIL && lowerRank == null
      && members.Count != 0)
      return false;

    foreach (var player in members) {
      player.GangRank = lowerRank?.Rank ?? null;
      player.GangId   = lowerRank == null ? null : player.GangId;
      await players.UpdatePlayer(player);
    }

    Ranks[gang] = gangRanks.Where(r => r.Rank != rank);
    return true;
  }

  public Task<bool> DeleteAllRanks(int gang) {
    if (!Ranks.ContainsKey(gang)) return Task.FromResult(false);
    Ranks.Remove(gang);
    return Task.FromResult(true);
  }

  public Task<bool> UpdateRank(int gang, IGangRank rank) {
    Ranks[gang] = Ranks[gang].Select(r => r.Rank == rank.Rank ? rank : r);
    return Task.FromResult(true);
  }
}