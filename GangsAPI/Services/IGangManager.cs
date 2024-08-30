using GangsAPI.Struct;

namespace GangsAPI.Services;

/// <summary>
/// A manager for gangs. Allows for the creation, retrieval, updating, and deletion of gangs.
/// The Gang Manager should not be used to manage perks or stats.
/// Use the respective <see cref="IPerkManager"/> and <see cref="IStatManager"/> to manage those.
/// </summary>
public interface IGangManager {
  /// <summary>
  /// Gets all gangs.
  /// </summary>
  /// <returns>The collection of all gangs, or an empty collection if there are none.</returns>
  Task<IEnumerable<IGang>> GetGangs();

  /// <summary>
  /// Gets a gang by its ID.
  /// </summary>
  /// <param name="id">The gang associated with the given id, or null if there is not one.</param>
  /// <returns></returns>
  Task<IGang?> GetGang(int id);

  /// <summary>
  /// Gets a gang by the steam ID of one of its members.
  /// In theory, a player could be in multiple gangs, but this method
  /// in which case this behavior is undefined.
  /// </summary>
  /// <param name="steam"></param>
  /// <returns></returns>
  Task<IGang?> GetGang(ulong steam);

  /// <summary>
  /// Pushes a gang to the database.
  /// Used for updating or creating gangs.
  /// </summary>
  /// <param name="gang"></param>
  /// <returns>True if the update was successful</returns>
  Task<bool> PushGang(IGang gang);

  /// <summary>
  /// Deletes a gang by its ID.
  /// </summary>
  /// <param name="id">The ID of the gang to delete.</param>
  /// <returns>True if deletion was succesful</returns>
  Task<bool> DeleteGang(int id);

  /// <summary>
  /// Creates a gang with the given name and owner.
  /// </summary>
  /// <param name="name">The name of the gang</param>
  /// <param name="owner">The owner of the gang</param>
  /// <returns>The newly created (and populated, specifically the id) gang.
  /// If there was an error, this method will return null.
  /// </returns>
  Task<IGang?> CreateGang(string name, ulong owner);

  Task<IGang?> CreateGang(string name, IGangPlayer player) {
    return CreateGang(name, player.Steam);
  }
}