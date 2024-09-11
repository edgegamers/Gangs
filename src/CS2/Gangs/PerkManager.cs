using GangsAPI;
using GangsAPI.Perks;
using GangsAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Stats.Perk;

namespace GangsImpl;

public class PerkManager : IPerkManager {
  public PerkManager(IServiceProvider provider) {
    Perks = new List<IPerk> { new GangChatPerk(provider) };
    foreach (var perk in Perks) {
      provider.GetRequiredService<IGangPlugin>()
       .Base.RegisterAllAttributes(perk);
    }
  }

  public IEnumerable<IPerk> Perks { get; }
}