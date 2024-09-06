using GangsAPI.Services;
using GangsAPI.Services.Commands;
using MartinCostello.Logging.XUnit;
using Microsoft.Extensions.DependencyInjection;
using Mock;

namespace GangsTest;

public class Startup {
  public void ConfigureServices(IServiceCollection services) {
    services.AddScoped<IPlayerManager, MockPlayerManager>();
    services.AddScoped<IGangManager, MockGangManager>();
    services.AddScoped<IStatManager, MockStatManager>();
    services.AddScoped<IGangStatManager, MockInstanceStatManager>();
    services.AddScoped<IPlayerStatManager, MockInstanceStatManager>();
    services.AddScoped<ICommandManager, MockCommandManager>();
  }
}