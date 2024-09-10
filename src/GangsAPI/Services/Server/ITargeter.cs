using GangsAPI.Data;
using Microsoft.Extensions.Localization;

namespace GangsAPI.Services.Server;

public interface ITargeter : IPluginBehavior {
  Task<IEnumerable<PlayerWrapper>> GetTarget(string query,
    PlayerWrapper? executor = null, IStringLocalizer? localizer = null);

  Task<PlayerWrapper?> GetSingleTarget(string query, out bool matchedMany,
    PlayerWrapper? executor = null, IStringLocalizer? localizer = null);
}