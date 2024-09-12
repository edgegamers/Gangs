using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Perks;
using GangsAPI.Services.Menu;
using Stats.Stat;

namespace Stats.Perk;

public abstract class BasePerk(IServiceProvider provider) : BaseStat, IPerk {
  public abstract int Cost { get; }
  public abstract Task OnPurchase(IGangPlayer player);

  public Task<IMenu?> GetMenu(IGangPlayer player, object value) {
    var menu = new BasicPerkMenu(provider, this);
    return Task.FromResult<IMenu>(menu)!;
  }
}

public abstract class BasePerk<TV>(IServiceProvider provider)
  : BasePerk(provider), IPerk, IStat<TV> {
  public abstract TV Value { get; set; }
  public override Type ValueType => typeof(TV);

  public bool Equals(IStat<TV>? other) {
    return other is not null && StatId == other.StatId;
  }
}