namespace Stats.Stat.Player;

public class PendingInvitations : BaseStat<PendingInvitationData> {
  public override string StatId => "pending_invitation";
  public override string Name => "Invitations";
  public override string? Description => "Invitations the player has received.";
  public override PendingInvitationData? Value { get; set; } = new();
}

public class PendingInvitationData {
  public string InvitingGangs { get; set; } = "";

  public List<int> GetInvitingGangs() {
    return InvitingGangs.Split(',', StringSplitOptions.RemoveEmptyEntries)
     .Select(int.Parse)
     .ToList();
  }

  public PendingInvitationData AddInvitation(int gangId) {
    InvitingGangs = string.Join(",", GetInvitingGangs().Append(gangId));
    return this;
  }

  public PendingInvitationData RemoveInvitation(int gangId) {
    InvitingGangs =
      string.Join(",", GetInvitingGangs().Where(id => id != gangId));
    return this;
  }
}