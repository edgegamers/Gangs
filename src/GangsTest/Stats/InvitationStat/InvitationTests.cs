using GangsAPI.Extensions;
using Stats.Stat;
using Stats.Stat.Gang;

namespace GangsTest.Stats.InvitationStat;

public class InvitationTests {
  private static readonly Random RNG = new();

  [Theory]
  [ClassData(typeof(TestData))]
  public void Invite_Dates(InvitationData data, int[] dates, ulong[] _1,
    ulong[] _2) {
    var timestamps =
      dates.Select(u => DateTime.UnixEpoch.AddSeconds(u)).ToList();
    Assert.Equal(timestamps, data.GetDates());
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public void Invite_Steams(InvitationData data, int[] _1, ulong[] invited,
    ulong[] _2) {
    Assert.Equal(invited, data.GetInvitedSteams());
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public void Invite_Inviter_Steams(InvitationData data, int[] _1, ulong[] _2,
    ulong[] inviter) {
    Assert.Equal(inviter, data.GetInviterSteams());
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public void Invite_Add_Retain_Previous(InvitationData data, int[] _1,
    ulong[] oInvitees, ulong[] oInviters) {
    var previousCount = data.GetDates().Count;
    var inviter       = new Random().NextULong();
    var invited       = new Random().NextULong();
    data.AddInvitation(inviter, invited);

    var dates         = data.GetDates();
    var inviterSteams = data.GetInviterSteams();
    var invitedSteams = data.GetInvitedSteams();

    // Ensure all old entries are still present
    for (var i = 0; i < previousCount; i++) {
      Assert.Contains(dates[i], dates);
      Assert.Contains(oInviters[i], inviterSteams);
      Assert.Contains(oInvitees[i], invitedSteams);
    }
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public void Invite_Add_Retain_Lineup(InvitationData data, int[] _1,
    ulong[] _2, ulong[] _3) {
    var inviter = RNG.NextULong();
    var invited = RNG.NextULong();
    data.AddInvitation(inviter, invited);

    var dates         = data.GetDates();
    var inviterSteams = data.GetInviterSteams();
    var invitedSteams = data.GetInvitedSteams();

    // Ensure all entries still line up
    foreach (var entry in data.GetEntries()) {
      var timestamp      = entry.Date;
      var timestampIndex = dates.IndexOf(timestamp);
      var inviterIndex   = inviterSteams.IndexOf(entry.Inviter);
      var invitedIndex   = invitedSteams.IndexOf(entry.Steam);
      Assert.Equal(timestampIndex, inviterIndex);
      Assert.Equal(timestampIndex, invitedIndex);
    }
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public void Invite_Add_Works(InvitationData data, int[] _1, ulong[] _2,
    ulong[] _3) {
    var now           = DateTime.Now;
    var previousCount = data.GetDates().Count;
    var inviter       = RNG.NextULong();
    var invited       = RNG.NextULong();
    data.AddInvitation(inviter, invited);

    var dates = data.GetDates();
    Assert.Equal(previousCount + 1, dates.Count);

    Assert.Contains(
      dates.Select(d => d.Subtract(DateTime.UnixEpoch).TotalSeconds),
      d => d > now.Subtract(DateTime.UnixEpoch).TotalSeconds - 1);
    Assert.Contains(inviter, data.GetInviterSteams());
    Assert.Contains(invited, data.GetInvitedSteams());
  }
}