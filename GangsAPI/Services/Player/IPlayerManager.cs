using GangsAPI.Data;
using GangsAPI.Data.Gang;

namespace GangsAPI.Services.Player;

/// <summary>
///   A manager for players. Allows for the retrieval and creation of players.
/// </summary>
public interface IPlayerManager : IPluginBehavior, ICacher {
  /// <summary>
  ///   Gets a player by their SteamID64.
  /// </summary>
  /// <param name="steamId">The SteamID64 of the player.</param>
  /// <returns>The player, or null if they did not exist.</returns>
  Task<IGangPlayer?> GetPlayer(ulong steamId) {
    return GetPlayer(steamId, false);
  }

  /// <summary>
  ///   Gets a player by their SteamID64.
  ///   If the player was already created, returns them.
  /// </summary>
  /// <param name="steamId">The SteamID64 of the player.</param>
  /// <param name="create">True if the manager should create a new player if they don't exist.</param>
  /// <returns>The player, or null if create is false, or an error occured creating one.</returns>
  async Task<IGangPlayer?> GetPlayer(ulong steamId, bool create) {
    var player = await GetPlayer(steamId);
    if (!create || player != null) return player;
    player = await CreatePlayer(steamId);
    return player;
  }

  /// <summary>
  ///   Creates a new player.
  /// </summary>
  /// <param name="steamId">The SteamID64 of the player.</param>
  /// <param name="name">The name of the player.</param>
  /// <returns>The new player.</returns>
  Task<IGangPlayer> CreatePlayer(ulong steamId, string? name = null);

  Task<bool> UpdatePlayer(IGangPlayer player);

  /// <summary>
  ///   Deletes a player and all of their associated data.
  /// </summary>
  /// <param name="steamId">The SteamID64 of the player.</param>
  /// <returns>True if the player was deleted, false otherwise.</returns>
  Task<bool> DeletePlayer(ulong steamId);
}