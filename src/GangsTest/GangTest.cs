using GangsAPI.Data;
using GangsAPI.Extensions;
using GangsAPI.Perks;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using GangsAPI.Services.Server;
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

  protected readonly IStringLocalizer Locale =
    provider.GetRequiredService<IStringLocalizer>();

  protected readonly IPlayerStatManager PlayerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  protected readonly IPlayerManager Players =
    provider.GetRequiredService<IPlayerManager>();

  protected readonly IServerProvider Server =
    provider.GetRequiredService<IServerProvider>();

  protected readonly IRankManager Ranks =
    provider.GetRequiredService<IRankManager>();
  
  protected readonly IGangChatPerk GangChat =
    provider.GetRequiredService<IGangChatPerk>();

  protected readonly PlayerWrapper TestPlayer = TestUtil.CreateFakePlayer();
}