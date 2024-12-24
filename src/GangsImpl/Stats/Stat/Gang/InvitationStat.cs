namespace Stats.Stat.Gang;

public class InvitationStat : BaseStat<InvitationData> {
  public const string STAT_ID = "gang_invitation";
  public override string StatId => STAT_ID;
  public override string Name => "Invitation";
  public override string Description => "Invitations sent or received.";
  public override InvitationData? Value { get; set; } = new();
}

public class InvitationData {
  /// <summary>
  ///   List of players the gang has invited
  /// </summary>
  public string InvitedSteams { get; set; } = "";

  /// <summary>
  ///   Linked list (to InvitedSteams) of players who invited the player
  /// </summary>
  public string InviterSteams { get; set; } = "";

  /// <summary>
  ///   Separate list of players who requested to join the gang
  /// </summary>
  public string RequestedSteams { get; set; } = "";

  public string Dates { get; set; } = "";
  public int MaxAmo { get; init; } = 5;

  public List<ulong> GetInvitedSteams() {
    return InvitedSteams.Split(",", StringSplitOptions.RemoveEmptyEntries)
     .Select(ulong.Parse)
     .ToList();
  }

  public List<ulong> GetInviterSteams() {
    return InviterSteams.Split(",", StringSplitOptions.RemoveEmptyEntries)
     .Select(ulong.Parse)
     .ToList();
  }

  public List<ulong> GetRequestedSteams() {
    return RequestedSteams.Split(",", StringSplitOptions.RemoveEmptyEntries)
     .Select(ulong.Parse)
     .ToList();
  }

  public List<DateTime> GetDates() {
    return Dates.Split(",", StringSplitOptions.RemoveEmptyEntries)
     .Select(double.Parse)
     .Select(u => DateTime.UnixEpoch.AddSeconds(u))
     .ToList();
  }

  public InvitationData AddInvitation(ulong inviter, ulong invited) {
    var now = DateTime.Now;
    InvitedSteams = string.Join(",", GetInvitedSteams().Append(invited));
    InviterSteams = string.Join(",", GetInviterSteams().Append(inviter));
    Dates = string.Join(",",
      GetDates()
       .Select(s => s.Subtract(DateTime.UnixEpoch).TotalSeconds)
       .Append((int)now.Subtract(DateTime.UnixEpoch).TotalSeconds));
    return this;
  }

  public bool RemoveInvitation(ulong invited) {
    var invitedSteams = GetInvitedSteams();
    var inviterSteams = GetInviterSteams();
    var dates         = GetDates();
    var index         = invitedSteams.IndexOf(invited);
    if (index == -1) return false;
    invitedSteams.RemoveAt(index);
    inviterSteams.RemoveAt(index);
    dates.RemoveAt(index);
    InvitedSteams = string.Join(",", invitedSteams);
    InviterSteams = string.Join(",", inviterSteams);
    Dates = string.Join(",",
      dates.Select(s => s.Subtract(DateTime.UnixEpoch).TotalSeconds));
    return true;
  }

  public InvitationData AddRequest(ulong requested) {
    RequestedSteams = string.Join(",", GetRequestedSteams().Append(requested));
    return this;
  }

  public InvitationData RemoveRequest(ulong requested) {
    var requestedSteams = GetRequestedSteams();
    var index           = requestedSteams.IndexOf(requested);
    if (index == -1) return this;
    requestedSteams.RemoveAt(index);
    RequestedSteams = string.Join(",", requestedSteams);
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