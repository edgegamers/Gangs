using System.Diagnostics;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Stats.Perk.Smoke;

public class SmokeColorPerk(IServiceProvider provider)
  : BasePerk<SmokePerkData>(provider) {
  public const string STAT_ID = "smoke_color";

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  public override string StatId => STAT_ID;
  public override string Name => "Smoke Color";

  public override string? Description
    => "Change the color of the smokes your gang throws!";

  public override SmokePerkData Value { get; set; } = new();

  public override Task<int?> GetCost(IGangPlayer player) {
    return Task.FromResult<int?>(null);
  }

  public override async Task<IMenu?> GetMenu(IGangPlayer player) {
    Debug.Assert(player.GangId != null, "player.GangId != null");
    var data =
      await gangStats.GetForGang<SmokePerkData>(player.GangId.Value, STAT_ID)
      ?? new SmokePerkData();
    return new SmokeColorMenu(Provider, data);
  }

  public override Task OnPurchase(IGangPlayer player) {
    return Task.CompletedTask;
  }
}

public class SmokePerkData {
  public SmokeColor Unlocked { get; set; }
  public SmokeColor Equipped { get; set; }
}