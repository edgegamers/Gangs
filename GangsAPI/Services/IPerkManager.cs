using GangsAPI.Struct;
using GangsAPI.Struct.Perk;

namespace GangsAPI.Services;

/// <summary>
/// A manager for perks. Allows for the registration, retrieval, and updating of perks.
/// </summary>
public interface IPerkManager : IPluginBehavior {
  /// <summary>
  /// Retrieves all perks.
  /// </summary>
  /// <returns></returns>
  Task<IEnumerable<IPerk>> GetPerks();

  /// <summary>
  /// Retrieves a perk by its ID.
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  Task<IPerk?> GetPerk(string id);

  /// <summary>
  /// Registers a perk with the manager.
  /// </summary>
  /// <param name="perk"></param>
  /// <returns></returns>
  Task<bool> RegisterPerk(IPerk perk);

  /// <summary>
  /// Unregisters a perk with the manager.
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  Task<bool> UnregisterPerk(string id);

  /// <summary>
  /// Updates a perk with the manager.
  /// </summary>
  /// <param name="perk"></param>
  /// <returns></returns>
  Task<bool> UpdatePerk(IPerk perk);

  /// <summary>
  /// Updates an instance of a perk with the manager.
  /// </summary>
  /// <param name="perk"></param>
  /// <typeparam name="K"></typeparam>
  /// <typeparam name="V"></typeparam>
  /// <returns></returns>
  Task<bool> UpdatePerk<K, V>(IPerk<K, V> perk);
}