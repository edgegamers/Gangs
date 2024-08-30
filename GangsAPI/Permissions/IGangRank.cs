namespace GangsAPI.Permissions;

public interface IGangRank {
  [Flags]
  public enum Permissions {
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
    ///   The member may manage the ranks of the gang, regardless
    ///   of rank system, the member will not be able to manage
    ///   their own rank.
    /// </summary>
    MANAGE_RANKS = 1 << 8,

    /// <summary>
    ///   The member may create new ranks for the gang.
    ///   All ranks created must not have a rank higher than
    ///   the member's current rank. Depending on the rank system,
    ///   these ranks may also be required to have a rank lower
    ///   than the member's current rank.
    /// </summary>
    CREATE_RANKS = 1 << 9,

    /// <summary>
    ///   The member has full access to all permissions.
    /// </summary>
    ADMINISTRATOR = 1 << 10,

    /// <summary>
    ///   The member is the owner of the gang, and can not be kicked.
    /// </summary>
    OWNER = 1 << 11
  }

  string Name { get; }
  int Rank { get; }
  Permissions Perms { get; }
}