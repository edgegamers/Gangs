using GangsAPI.Extensions;
using GangsAPI.Perks;
using Microsoft.Extensions.DependencyInjection;

namespace Stats.Perk;

public static class PerkCollection {
  public static void RegisterPerks(this IServiceCollection provider) {
    provider.AddPluginBehavior<IGangChatPerk, GangChatPerk>();
  }
}