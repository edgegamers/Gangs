using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Commands;
using GangsAPI;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace GangsImpl;

public class GangPlugin(IServiceProvider provider) : BasePlugin, IGangPlugin {
  public override string ModuleName => "Gangs";
  public override string ModuleVersion => "0.0.1";
  public BasePlugin Base => this;
  public IServiceProvider Services { get; } = provider;
}