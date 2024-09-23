using GangsAPI.Data;
using GangsAPI.Extensions;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace GangsTest;

public abstract class GangTest(IServiceProvider provider) {
  protected readonly ICommandManager Commands =
    provider.GetRequiredService<ICommandManager>();

  protected readonly IGangManager Gangs =
    provider.GetRequiredService<IGangManager>();

  protected readonly IGangStatManager GangStats =
    provider.GetRequiredService<IGangStatManager>();

  protected readonly PlayerWrapper TestPlayer =
    new(new Random().NextULong(), "Test Player");

  protected readonly IStringLocalizer Locale =
    provider.GetRequiredService<IStringLocalizer>();

  protected readonly IPlayerStatManager PlayerStats =
    provider.GetRequiredService<IPlayerStatManager>();
}