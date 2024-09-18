using GangsAPI.Data.Gang;

namespace Stats.Perk.Smoke;

public class SmokeColorPerk(IServiceProvider provider)
  : BasePerk<SmokePerkData>(provider) {
  public override string StatId => STAT_ID;
  public override string Name => "Smoke Color";
  public const string STAT_ID = "smoke_color";

  public override string? Description
    => "Change the color of the smokes your gang throws!";

  public override Task<int?> GetCost(IGangPlayer player) {
    return Task.FromResult<int?>(null);
  }

  public override Task OnPurchase(IGangPlayer player) {
    return Task.CompletedTask;
  }

  public override SmokePerkData Value { get; set; } = new();
}

public class SmokePerkData {
  public SmokeColor Unlocked { get; set; }
  public SmokeColor Equipped { get; set; }
}