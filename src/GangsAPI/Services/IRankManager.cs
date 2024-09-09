using GangsAPI.Data.Gang;
using GangsAPI.Permissions;

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
  /// All players that had the rank originally will be demoted
  /// to their immediate lower rank.
  /// </summary>
  /// <param name="gang"></param>
  /// <param name="rank"></param>
  /// <returns></returns>
  Task<bool> DeleteRank(int gang, int rank);

  /// <summary>
  /// Updates the name and permissions of a rank.
  /// Updating the rank number is not allowed, and can
  /// only be done by deleting the rank and creating a new one.
  /// </summary>
  /// <param name="gang"></param>
  /// <param name="rank"></param>
  /// <returns>
  /// True if the rank was updated successfully, false otherwise.
  /// (Rank did not exist, rank is the owner rank, etc.)
  /// </returns>
  Task<bool> UpdateRank(int gang, IGangRank rank) => AddRank(gang, rank);

  /// <summary>
  /// Populates a gang with default ranks. At minimum this must
  /// include the owner rank.
  /// </summary>
  /// <returns></returns>
  async Task<IEnumerable<IGangRank>> AssignDefaultRanks(int gang) {
    var owner = await CreateRank(gang, "Owner", 0, IGangRank.Permissions.OWNER);
    if (owner == null)
      throw new InvalidOperationException("Failed to create owner rank.");
    return new[] { owner };
  }

  #region Aliases

  Task<bool> AddRank(IGang gang, IGangRank rank) => AddRank(gang.GangId, rank);

  Task<IGangRank?> GetRank(IGang gang, int rank) => GetRank(gang.GangId, rank);

  Task<bool> DeleteRank(int gang, IGangRank rank)
    => DeleteRank(gang, rank.Rank);

  Task<IEnumerable<IGangRank>> GetRanks(IGang gang) => GetRanks(gang.GangId);

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