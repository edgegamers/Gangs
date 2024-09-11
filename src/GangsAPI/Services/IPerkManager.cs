using GangsAPI.Perks;

namespace GangsAPI.Services;

public interface IPerkManager : IPluginBehavior {
  IEnumerable<IPerk> Perks { get; }
}