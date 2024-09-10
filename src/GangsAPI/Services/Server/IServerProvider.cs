using GangsAPI.Data;

namespace GangsAPI;

public interface IServerProvider : IPluginBehavior {
  Task<IReadOnlyList<PlayerWrapper>> GetPlayers();
  Task AddPlayer(PlayerWrapper player);
  Task<bool> RemovePlayer(PlayerWrapper player);
}