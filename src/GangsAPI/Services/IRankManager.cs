using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services.Player;

namespace GangsAPI.Services;

/// <summary>
/// A manager for ranks
/// </summary>
public interface IRankManager : IPluginBehavior {
  /// <summary>
  /// Requests all currently existing ranks from all existing gangs.
  /// Not sure why you'd need to call this.
  /// </summary>
  /// <returns></returns>
  Task<Dictionary<int, IEnumerable<IGangRank>>> GetAllRanks();

  /// <summary>
  /// Gets all defined ranks for a specific gang.
  /// All gangs must have at least one rank, the owner rank.
  /// </summary>
  /// <param name="gang"></param>
  /// <returns></returns>
  Task<IEnumerable<IGangRank>> GetRanks(int gang);

  /// <summary>
  /// Gets a specific rank from a specific gang.
  /// </summary>
  /// <param name="gang"></param>
  /// <param name="rank"></param>
  /// <returns></returns>
  Task<IGangRank?> GetRank(int gang, int rank);

  /// <summary>
  /// Attempts to add a rank to a specific gang.
  /// If a rank already exists with the same rank number,
  ///  this will return false.
  /// If the rank has a negative rank, this will return false.
  /// </summary>
  /// <param name="gang"></param>
  /// <param name="rank"></param>
  /// <returns></returns>
  Task<bool> AddRank(int gang, IGangRank rank);

  /// <summary>
  /// Attempts to create a rank for a specific gang.
  /// </summary>
  /// <param name="gang"></param>
  /// <param name="name"></param>
  /// <param name="rank"></param>
  /// <param name="permissions"></param>
  /// <returns></returns>
  Task<IGangRank?> CreateRank(int gang, string name, int rank,
    IGangRank.Permissions permissions);

  /// <summary>
  /// Deletes a rank from a specific gang.
  /// If an error occured, this will return false.
  /// If the rank is the owner rank, this will return false.
  /// If force is false and any members have this rank, this will return false.
  /// If force is true, the the manager is required to ensure that
  /// the <see cref="IPlayerManager"/> is aware of the rank deletion.
  /// This may require demoting all players with the rank to a lower rank,
  /// or removing those players from the gang entirely (if a lower rank does not exist). 
  /// </summary>
  /// <param name="gang"></param>
  /// <param name="rank"></param>
  /// <param name="force"></param>
  /// <returns></returns>
  Task<bool> DeleteRank(int gang, int rank, DeleteStrat strat);

  public enum DeleteStrat {
    /// <summary>
    /// Cancel the deletion of the rank if any
    /// players currently have the rank.
    /// </summary>
    CANCEL,

    /// <summary>
    /// Attempt to demote all players with the rank,
    /// if a lower rank does not exist, cancel the deletion.
    /// </summary>
    DEMOTE_FAIL,

    /// <summary>
    /// Attempt to demote all players with the rank,
    /// if a lower rank does not exist, kick the player from the gang.
    /// </summary>
    DEMOTE_KICK
  }

  /// <summary>
  /// Deletes all ranks of a specific gang,
  /// this should only be used when deleting a gang.
  /// </summary>
  /// <param name="gang"></param>
  /// <returns></returns>
  Task<bool> DeleteAllRanks(int gang);

  /// <summary>
  /// Updates the name and permissions of a rank.
  /// Updating the rank number is not allowed, and can
  /// only be done by deleting the rank and creating a new one.
  /// </summary>
  /// <param name="gang"></param>
  /// <param name="rank"></param>
  /// <returns>
  /// True if the rank was updated successfully, false otherwise.
  /// </returns>
  Task<bool> UpdateRank(int gang, IGangRank rank);

  /// <summary>
  /// Populates a gang with default ranks. At minimum this must
  /// include the owner rank.
  /// </summary>
  /// <returns></returns>
  async Task<IEnumerable<IGangRank>> AssignDefaultRanks(int gang) {
    var memberPerms = IGangRank.Permissions.BANK_DEPOSIT
      | IGangRank.Permissions.VIEW_MEMBERS;
    var officerPerms = memberPerms | IGangRank.Permissions.INVITE_OTHERS
      | IGangRank.Permissions.PURCHASE_PERKS
      | IGangRank.Permissions.BANK_WITHDRAW | IGangRank.Permissions.KICK_OTHERS;
    var managerPerms = officerPerms | IGangRank.Permissions.MANAGE_PERKS
      | IGangRank.Permissions.MANAGE_RANKS
      | IGangRank.Permissions.MANAGE_INVITES
      | IGangRank.Permissions.PROMOTE_OTHERS
      | IGangRank.Permissions.DEMOTE_OTHERS;
    var coOwnerPerms = managerPerms | IGangRank.Permissions.CREATE_RANKS;

    var defaultRanks = new[] {
      await CreateRank(gang, "Owner", 0, IGangRank.Permissions.OWNER)
      ?? throw new InvalidOperationException("Failed to create owner rank."),
      await CreateRank(gang, "Co-Owner", 10, coOwnerPerms)
      ?? throw new InvalidOperationException("Failed to create co-owner rank."),
      await CreateRank(gang, "Manager", 30, managerPerms)
      ?? throw new InvalidOperationException("Failed to create manager rank."),
      await CreateRank(gang, "Officer", 50, officerPerms)
      ?? throw new InvalidOperationException("Failed to create officer rank."),
      await CreateRank(gang, "Member", 100, memberPerms)
      ?? throw new InvalidOperationException("Failed to create member rank.")
    };
    if (defaultRanks.Any(r => r == null))
      throw new InvalidOperationException("Failed to create default ranks.");
    return defaultRanks;
  }

  #region Aliases

  Task<bool> AddRank(IGang gang, IGangRank rank) => AddRank(gang.GangId, rank);

  Task<IGangRank?> GetRank(IGang gang, int rank) => GetRank(gang.GangId, rank);

  Task<bool> DeleteRank(int gang, IGangRank rank, DeleteStrat strat)
    => DeleteRank(gang, rank.Rank, strat);

  Task<IEnumerable<IGangRank>> GetRanks(IGang gang) => GetRanks(gang.GangId);

  Task<IEnumerable<IGangRank>> AssignDefaultRanks(IGang gang)
    => AssignDefaultRanks(gang.GangId);

  Task<bool> DeleteAllRanks(IGang gang) => DeleteAllRanks(gang.GangId);

  #endregion

  /// <summary>
  /// Helper method to get the owner rank of a gang.
  /// </summary>
  /// <param name="gang"></param>
  /// <returns></returns>
  async Task<string> GetOwnerName(IGang gang) {
    var rank = await GetRank(gang.GangId, 0);
    return rank?.Name ?? "Owner";
  }
}