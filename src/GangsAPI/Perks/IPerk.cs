using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Services.Menu;

namespace GangsAPI.Perks;

public interface IPerk : IStat {
  int GetCost(object value);
  Task OnPurchase(IGangPlayer player);
  Task<IMenu?> GetMenu(IGangPlayer player, object value);
}

public interface IPerk<T> : IPerk, IStat<T>, IPluginBehavior { }