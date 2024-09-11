using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Services.Menu;

namespace GangsAPI.Perks;

public interface IPerk : IStat {
  int Cost { get; }
  Task OnPurchase(IGangPlayer player);
  Task<IMenu?> GetMenu(IGangPlayer player);
}

public interface IPerk<T> : IPerk, IStat<T>, IPluginBehavior { }