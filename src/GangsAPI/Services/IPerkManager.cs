using GangsAPI.Perks;

namespace GangsAPI.Services;

public interface IPerkManager : IPluginBehavior {
  List<IPerk> Perks { get; set; }
}