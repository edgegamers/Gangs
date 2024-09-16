using GangsAPI.Extensions;

namespace GangsAPI.Permissions;

/// <summary>
///   Represents a customizable rank that gang owners / admins may dictate.
///   Ranks are identified by their (gangId, rank) pair, and must be unique.
///   Rank hierarchy is dictated by rank number, with lower numbers being higher ranks.
///   (i.e. rank 0 (owner) is higher than rank 1 (admin), which is higher than rank 2 (member))
///   This means rank 0 **must** be the owner rank, and all ranks must be non-negative.
/// </summary>
[Flags]
public enum Perm {
  NONE = 0,

  /// <summary>
  ///   The member may invite others to the gang.
  /// </summary>
  INVITE_OTHERS = 1 << 0,

  /// <summary>
  ///   The member may kick others from the gang.
  /// </summary>
  KICK_OTHERS = 1 << 1,

  /// <summary>
  ///   The member may deposit money into the gang bank.
  ///   This also allows the member to use their own personal
  ///   funds to purchase perks for the gang.
  /// </summary>
  BANK_DEPOSIT = 1 << 2,

  /// <summary>
  ///   The member may withdraw money from the gang bank.
  ///   This also allows the member to use the gang's funds
  ///   to purchase perks for the gang.
  /// </summary>
  BANK_WITHDRAW = 1 << 3,

  /// <summary>
  ///   The member may promote others, the maximum rank that
  ///   they may promote to is determined by the rank system.
  /// </summary>
  PROMOTE_OTHERS = 1 << 4,

  /// <summary>
  ///   The member may demote others.
  /// </summary>
  DEMOTE_OTHERS = 1 << 5,

  /// <summary>
  ///   The member may purchase perks for the gang.
  /// </summary>
  PURCHASE_PERKS = 1 << 6,

  /// <summary>
  ///   The member may manage or configure perks for the gang.
  /// </summary>
  MANAGE_PERKS = 1 << 7,

  /// <summary>
  ///   The member may manage the ranks (names + perms) of the gang, regardless
  ///   of rank system, the member will not be able to manage
  ///   their own rank, or create other ranks.
  ///   The member may not grant any permissions that they themselves do not have.
  /// </summary>
  MANAGE_RANKS = 1 << 8,

  /// <summary>
  ///   The member may create new ranks for the gang.
  ///   All ranks created must not have a rank higher than
  ///   the member's current rank.
  /// </summary>
  CREATE_RANKS = 1 << 9,

  /// <summary>
  ///   The member has full access to all permissions.
  /// </summary>
  ADMINISTRATOR = 1 << 10 | INVITE_OTHERS | KICK_OTHERS | BANK_DEPOSIT
    | BANK_WITHDRAW | PROMOTE_OTHERS | DEMOTE_OTHERS | PURCHASE_PERKS
    | MANAGE_PERKS | MANAGE_RANKS | CREATE_RANKS | VIEW_MEMBER_DETAILS
    | MANAGE_INVITES,

  /// <summary>
  ///   The member is the owner of the gang, and can not be kicked.
  /// </summary>
  OWNER = 1 << 11 | ADMINISTRATOR,

  /// <summary>
  ///   The member may view detailed information of other
  ///   gang members. (Last logged in, kills, etc.)
  /// </summary>
  VIEW_MEMBER_DETAILS = 1 << 12,

  /// <summary>
  ///   The member may manage invites to the gang.
  /// </summary>
  MANAGE_INVITES = 1 << 13 | INVITE_OTHERS
}

public interface IGangRank : IComparable<IGangRank> {
  string Name { get; }
  int Rank { get; }
  Perm Permissions { get; set; }
}

public static class PermissionExtensions {
  public static string ToFriendlyString(this Perm perms) {
    return perms switch {
      Perm.INVITE_OTHERS       => "Invite Others",
      Perm.KICK_OTHERS         => "Kick Others",
      Perm.BANK_DEPOSIT        => "Deposit Money",
      Perm.BANK_WITHDRAW       => "Withdraw Money",
      Perm.PROMOTE_OTHERS      => "Promote Others",
      Perm.DEMOTE_OTHERS       => "Demote Others",
      Perm.PURCHASE_PERKS      => "Purchase Perks",
      Perm.MANAGE_PERKS        => "Manage Perks",
      Perm.MANAGE_RANKS        => "Manage Ranks",
      Perm.CREATE_RANKS        => "Create Ranks",
      Perm.ADMINISTRATOR       => "Administrator",
      Perm.OWNER               => "Owner",
      Perm.VIEW_MEMBER_DETAILS => "View Member Details",
      Perm.MANAGE_INVITES      => "Manage Invites",
      _                        => perms.ToString().ToTitleCase()
    };
  }

  public static string Describe(this Perm perms) {
    if (perms.HasFlag(Perm.OWNER)) return "Owner";
    if (perms.HasFlag(Perm.ADMINISTRATOR)) return "Administrator";

    var permissions =
      (from perm in Enum.GetValues<Perm>()
        where perms.HasFlag(perm) && perm != Perm.NONE
        select ToFriendlyString(perm)).ToList();

    switch (permissions.Count) {
      case 0:
        return "No permissions";
      case 1:
        return permissions[0] ?? string.Empty;
      case 2:
        return $"{permissions[0]} and {permissions[1]}";
    }

    // Oxford comma
    var last = permissions[^1];
    permissions.RemoveAt(permissions.Count - 1);
    return $"{string.Join(", ", permissions)}, and {last}";
  }
}