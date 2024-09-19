using GangsAPI.Data;
using Microsoft.Extensions.Localization;

namespace GangsAPI.Services.Server;

public interface IPlayerTargeter : IPluginBehavior {
  Task<IEnumerable<PlayerWrapper>> GetTarget(string query,
    PlayerWrapper? executor = null, IStringLocalizer? localizer = null);

  Task<PlayerWrapper?> GetSingleTarget(string query,
    PlayerWrapper? executor = null, IStringLocalizer? localizer = null);
}