using GangsAPI.Data.Gang;
using GangsAPI.Perks;
using GangsAPI.Services.Menu;
using Stats.Stat;

namespace Stats.Perk;

public abstract class BasePerk(IServiceProvider? provider) : BaseStat, IPerk {
  public abstract int Cost { get; }
  public abstract Task OnPurchase(IGangPlayer player);

  public Task<IMenu?> GetMenu(IGangPlayer player) {
    if (provider == null) return Task.FromResult<IMenu?>(null);
    var menu = new BasicPerkMenu(provider, this);
    return Task.FromResult<IMenu>(menu)!;
  }
}

public abstract class BasePerk<TV>(IServiceProvider? provider)
  : BaseStat, IPerk {
  public abstract int Cost { get; }
  public abstract Task OnPurchase(IGangPlayer player);

  public Task<IMenu?> GetMenu(IGangPlayer player) {
    if (provider == null) return Task.FromResult<IMenu?>(null);
    var menu = new BasicPerkMenu(provider, this);
    return Task.FromResult<IMenu>(menu)!;
  }

  public override Type ValueType => typeof(TV);
}