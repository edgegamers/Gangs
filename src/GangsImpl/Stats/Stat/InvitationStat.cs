using GangsAPI.Data.Stat;

namespace Stats;

public class InvitationStat : BaseStat<InvitationData> {
  public override string StatId => "gang_invitation";
  public override string Name => "Invitation";
  public override string Description => "Invitations sent or received.";

  public override IStat<InvitationData?> Clone() {
    return new InvitationStat { Value = Value };
  }
}

public class InvitationData {
  public string InvitedSteams { get; set; } = "";
  public string InviterSteams { get; set; } = "";
  public string Dates { get; set; } = "";
  public int MaxAmo { get; init; } = 5;

  public List<ulong> GetInvitedSteams() {
    return InvitedSteams == "" ? [] :
      InvitedSteams.Trim(',').Split(",").Select(ulong.Parse).ToList();
  }

  public List<ulong> GetInviterSteams() {
    return InviterSteams == "" ? [] :
      InviterSteams.Trim(',').Split(",").Select(ulong.Parse).ToList();
  }

  public List<DateTime> GetDates() {
    if (Dates == "") return [];
    return Dates.Trim(',')
     .Split(",")
     .Select(ulong.Parse)
     .Select(u => DateTime.UnixEpoch.AddSeconds(u))
     .ToList();
  }

  public InvitationData AddInvitation(ulong inviter, ulong invited) {
    var now = DateTime.Now;
    InvitedSteams += $"{invited},";
    InviterSteams += $"{inviter},";
    Dates         += $"{(ulong)now.Subtract(DateTime.UnixEpoch).TotalSeconds},";
    return this;
  }

  public List<InvitationEntry> GetEntries() {
    var steams        = GetInvitedSteams();
    var dates         = GetDates();
    var inviterSteams = GetInviterSteams();

    return steams.Select((steam, index) => new InvitationEntry {
        Steam = steam, Inviter = inviterSteams[index], Date = dates[index]
      })
     .ToList();
  }
}

public struct InvitationEntry {
  public required ulong Steam { get; init; }
  public required ulong Inviter { get; init; }
  public required DateTime Date { get; init; }
}