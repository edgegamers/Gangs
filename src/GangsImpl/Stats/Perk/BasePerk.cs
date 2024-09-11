using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Perks;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Menu;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat;

namespace Stats.Perk;

public abstract class BasePerk(IServiceProvider? provider) : BaseStat, IPerk {
  public abstract int Cost { get; }
  public abstract Task OnPurchase(IGangPlayer player);

  public Task<IMenu>? GetMenu(IGangPlayer player) {
    if (provider == null) return null;
    var menu = new BasicPerkMenu(provider, this);
    return Task.FromResult<IMenu>(menu);
  }
}