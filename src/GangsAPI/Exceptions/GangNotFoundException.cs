using GangsAPI.Data.Gang;

namespace GangsAPI.Exceptions;

public class GangNotFoundException : GangException {
  public GangNotFoundException(IGangPlayer player) : base(
    $"Could not find gang for player {player.Steam} using associated gang ID {player.GangId} (Rank: {player.GangRank}") { }

  public GangNotFoundException(int gangId) : base(
    $"Could not find gang with ID {gangId}") { }
}