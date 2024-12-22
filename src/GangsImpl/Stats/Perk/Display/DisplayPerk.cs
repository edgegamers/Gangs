using System.Diagnostics;
using GangsAPI.Data.Gang;
using GangsAPI.Perks;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Stats.Perk.Display;

public class DisplayPerk(IServiceProvider provider)
  : BasePerk<DisplayData>(provider), IDisplayPerk {
  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  public override DisplayData Value { get; set; } = new();
  public int ChatCost => 50000;
  public int ScoreboardCost => 5000;
  public override string StatId => "display_perk";
  public override string Name => "Display";

  public override string Description
    => "Customize where your gang name shows up";

  public override Task<int?> GetCost(IGangPlayer player) {
    return Task.FromResult<int?>(null);
  }

  public override Task OnPurchase(IGangPlayer player) {
    return Task.CompletedTask;
  }

  public override async Task<IMenu?> GetMenu(IGangPlayer player) {
    Debug.Assert(player.GangId != null, "player.GangId != null");
    var (_, data) =
      await gangStats.GetForGang<DisplayData>(player.GangId.Value, StatId);
    var menu = new DisplayPerkMenu(Provider, data ?? new DisplayData());
    return menu;
  }

  public async Task<bool> HasChatDisplay(IGang gang) {
    var (success, data) = await gangStats.GetForGang<DisplayData>(gang, StatId);
    return success && data is { ChatBought: true };
  }

  public async Task<bool> HasScoreboardDisplay(IGang gang) {
    var (success, data) = await gangStats.GetForGang<DisplayData>(gang, StatId);
    return success && data is { ScoreboardBought: true };
  }

  public async Task SetChatDisplay(IGang gang, bool value) {
    var (success, data) = await gangStats.GetForGang<DisplayData>(gang, StatId);
    if (!success || data == null) data = new DisplayData();
    data.ChatBought = value;
    await gangStats.SetForGang(gang, StatId, data);
  }

  public async Task SetScoreboardDisplay(IGang gang, bool value) {
    var (success, data) = await gangStats.GetForGang<DisplayData>(gang, StatId);
    if (!success || data == null) data = new DisplayData();
    data.ScoreboardBought = value;
    await gangStats.SetForGang(gang, StatId, data);
  }
}

public class DisplayData {
  public bool ChatBought { get; set; }
  public bool ScoreboardBought { get; set; }
}