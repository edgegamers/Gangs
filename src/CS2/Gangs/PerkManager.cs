using CounterStrikeSharp.API.Core;
using GangsAPI.Perks;
using GangsAPI.Services;
using Stats.Perk;
using Stats.Perk.Display;

namespace GangsImpl;

public class PerkManager(IServiceProvider provider) : IPerkManager {
  public void Start(BasePlugin? plugin, bool hotReload) {
    Perks = [
      new GangChatPerk(provider), new MotdPerk(provider),
      new CapacityPerk(provider), new DisplayPerk(provider)
    ];
  }

  public List<IPerk> Perks { get; set; } = [];
}