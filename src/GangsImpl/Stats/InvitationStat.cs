using GangsAPI.Data.Stat;

namespace Stats;

public class InvitationStat : BaseStat<string> {
  public override string StatId => "gang_invitation";
  public override string Name => "Invitation";
  public override string Description => "Invitations sent or received.";

  public List<ulong> GetAsSteams() {
    return Value?.Split(",").Select(ulong.Parse).ToList() ?? [];
  }

  public List<int> GetAsGangIds() {
    return Value?.Split(",").Select(int.Parse).ToList() ?? [];
  }

  public override IStat<string?> Clone() {
    return new InvitationStat { Value = Value };
  }
}