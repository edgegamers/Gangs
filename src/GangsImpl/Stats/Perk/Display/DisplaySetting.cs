using GangsAPI.Perks;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat;

namespace Stats.Perk.Display;

public class DisplaySetting(IServiceProvider provider)
  : BaseStat<DisplaySettingData>, IDisplaySetting {
  private readonly IPlayerStatManager playerStats =
    provider.GetRequiredService<IPlayerStatManager>();

  public override string StatId => "display_setting";
  public override string Name => "Display Setting";

  public override string Description
    => "Customize where your gang name shows up";

  public override DisplaySettingData? Value { get; set; } = new();

  public async Task<bool> IsChatEnabled(ulong steam) {
    var (success, data) =
      await playerStats.GetForPlayer<DisplaySettingData>(steam, StatId);

    return success && data is { ChatVisible: true };
  }

  public async Task<bool> IsScoreboardEnabled(ulong steam) {
    var (success, data) =
      await playerStats.GetForPlayer<DisplaySettingData>(steam, StatId);

    return success && data is { ScoreboardVisible: true };
  }

  public async Task SetChatEnabled(ulong steam, bool enabled) {
    var (success, data) =
      await playerStats.GetForPlayer<DisplaySettingData>(steam, StatId);
    if (!success || data == null) data = new DisplaySettingData();
    data.ChatVisible = enabled;
    await playerStats.SetForPlayer(steam, StatId, data);
  }

  public async Task SetScoreboardEnabled(ulong steam, bool enabled) {
    var (success, data) =
      await playerStats.GetForPlayer<DisplaySettingData>(steam, StatId);
    if (!success || data == null) data = new DisplaySettingData();
    data.ScoreboardVisible = enabled;
    await playerStats.SetForPlayer(steam, StatId, data);
  }
}

public class DisplaySettingData {
  public bool ChatVisible { get; set; } = true;
  public bool ScoreboardVisible { get; set; } = true;
}