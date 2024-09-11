using CounterStrikeSharp.API.Core;
using GangsAPI.Perks;
using GangsAPI.Services;
using Stats.Perk;

namespace GangsImpl;

public class PerkManager(IServiceProvider provider) : IPerkManager {
  public void Start(BasePlugin? plugin, bool hotReload) {
    Perks = new List<IPerk> { new GangChatPerk(provider) };
    foreach (var perk in Perks) plugin?.RegisterAllAttributes(perk);
  }

  public IEnumerable<IPerk> Perks { get; private set; } = [];
}