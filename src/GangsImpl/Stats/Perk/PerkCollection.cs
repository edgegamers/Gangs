using GangsAPI.Extensions;
using GangsAPI.Perks;
using Microsoft.Extensions.DependencyInjection;
using Stats.Perk.Display;

namespace Stats.Perk;

public static class PerkCollection {
  public static void RegisterPerks(this IServiceCollection provider) {
    provider.AddPluginBehavior<IGangChatPerk, GangChatPerk>();
    provider.AddPluginBehavior<ICapacityPerk, CapacityPerk>();
    provider.AddPluginBehavior<IMotdPerk, MotdPerk>();
    provider.AddScoped<IDisplayPerk, DisplayPerk>();
    provider.AddPluginBehavior<DisplayListener>();
  }
}