using GangsAPI.Data.Gang;

namespace GangsAPI.Exceptions;

public class RankNotFoundException : GangException {
  public RankNotFoundException(int gang, int rank) : base(
    $"Could not find rank {rank} in gang {gang}") { }

  public RankNotFoundException(IGangPlayer player) : base(
    $"Could not find rank {player.GangRank} in gang {player.GangId}") { }
}