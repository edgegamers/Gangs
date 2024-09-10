using GangsAPI.Data;

namespace GangsAPI;

public interface ITargeter : IPluginBehavior {
  Task<IEnumerable<PlayerWrapper>> GetTarget(string query,
    PlayerWrapper? executor = null);

  Task<PlayerWrapper?> GetSingleTarget(string query, out bool matchedMany,
    PlayerWrapper? executor = null );
}