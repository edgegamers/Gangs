using CounterStrikeSharp.API;
using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Perks;
using GangsAPI.Services.Menu;
using Stats.Stat;

namespace Stats.Perk;

public abstract class BasePerk(IServiceProvider provider) : BaseStat, IPerk {
  public abstract Task<int?> GetCost(IGangPlayer player);
  public abstract Task OnPurchase(IGangPlayer player);

  public virtual Task<IMenu?> GetMenu(IGangPlayer player) {
    var menu = new BasicPerkMenu(provider, this);
    return Task.FromResult<IMenu>(menu)!;
  }
}

public abstract class BasePerk<TV>(IServiceProvider provider)
  : BasePerk(provider), IPerk, IStat<TV> {
  public override Type ValueType => typeof(TV);
  public abstract TV Value { get; set; }

  public bool Equals(IStat<TV>? other) {
    return other is not null && StatId == other.StatId;
  }
}