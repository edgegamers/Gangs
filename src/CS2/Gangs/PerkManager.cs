using CounterStrikeSharp.API.Core;
using GangsAPI.Perks;
using GangsAPI.Services;
using Stats.Perk;
using Stats.Perk.Display;

namespace GangsImpl;

public class PerkManager(IServiceProvider provider) : IPerkManager {
  public void Start(BasePlugin? plugin, bool hotReload) {
    Perks = new List<IPerk> {
      new GangChatPerk(provider),
      new MotdPerk(provider),
      new CapacityPerk(provider),
      new DisplayPerk(provider)
    };
  }

  public IEnumerable<IPerk> Perks { get; private set; } = [];
}