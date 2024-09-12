using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Services.Menu;

namespace GangsAPI.Perks;

public interface IPerk : IStat {
  Task<int?> GetCost(IGangPlayer player);
  Task OnPurchase(IGangPlayer player);
  Task<IMenu?> GetMenu(IGangPlayer player);
}

public interface IPerk<T> : IPerk, IStat<T>, IPluginBehavior { }