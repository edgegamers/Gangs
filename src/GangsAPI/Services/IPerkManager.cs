using GangsAPI.Data.Stat;

namespace GangsAPI.Services;

public interface IPerkManager {
  Task<IEnumerable<IPerk>> GetRegisteredPerks();
  Task RegisterPerk(IPerk perk);
}