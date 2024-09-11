using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;

namespace GangsAPI.Perks;

public interface IPerk : IStat {
  int Cost { get; }
  Task OnPurchase(IGangPlayer player);
}

public interface IPerk<T> : IPerk, IStat<T>, IPluginBehavior { }