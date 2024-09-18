using System.Diagnostics.CodeAnalysis;
using GangsAPI.Data.Gang;

namespace GangsAPI.Services.Player;

/// <summary>
///   A manager for players. Allows for the retrieval and creation of players.
/// </summary>
public interface IPlayerManager : IPluginBehavior {
  /// <summary>
  ///   Gets a player by their SteamID64.
  ///   If the player does not exist, attempts to create them.
  /// </summary>
  /// <param name="steamId">The SteamID64 of the player.</param>
  /// <param name="create"></param>
  /// <returns>The player, or null if they did not exist.</returns>
  Task<IGangPlayer?> GetPlayer(ulong steamId, bool create = true);

  /// <summary>
  ///   Creates a new player.
  /// </summary>
  /// <param name="steamId">The SteamID64 of the player.</param>
  /// <param name="name">The name of the player.</param>
  /// <returns>The new player.</returns>
  Task<IGangPlayer> CreatePlayer(ulong steamId, string? name = null);

  Task<IEnumerable<IGangPlayer>> GetAllPlayers();

  Task<IEnumerable<IGangPlayer>> GetMembers(int gangId);

  Task<IEnumerable<IGangPlayer>> GetMembers(IGang gang) {
    return GetMembers(gang.GangId);
  }

  async Task<IGangPlayer?> SearchPlayer(IGang gang, string query)
    => await SearchPlayer(gang.GangId, query);

  async Task<IGangPlayer?> SearchPlayer(int gangId, string query) {
    var members = (await GetMembers(gangId)).ToList();

    var matchedSteams = members.Where(p => query.Contains(p.Steam.ToString()))
     .ToList();

    if (matchedSteams.Count == 0) return matchedSteams.First();

    var matchedNames = members.Where(p
        => p.Name != null
        && query.Contains(p.Name, StringComparison.OrdinalIgnoreCase))
     .ToList();

    return matchedNames.Count != 1 ? null : matchedNames.First();
  }

  Task<bool> UpdatePlayer(IGangPlayer player);

  /// <summary>
  ///   Deletes a player and all of their associated data.
  /// </summary>
  /// <param name="steamId">The SteamID64 of the player.</param>
  /// <returns>True if the player was deleted, false otherwise.</returns>
  Task<bool> DeletePlayer(ulong steamId);
}