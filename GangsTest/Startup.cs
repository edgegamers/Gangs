using GangsAPI.Services;
using GangsImpl.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace GangsTest;

public class Startup {
  public void ConfigureServices(IServiceCollection services) {
    services.AddSingleton<IPlayerManager, MockPlayerManager>();
    services.AddSingleton<IGangManager, MockGangManager>();
    services.AddSingleton<IStatManager, MockStatManager>();
    services.AddSingleton<IGangStatManager, MockInstanceStatManager>();
    services.AddSingleton<IPlayerStatManager, MockInstanceStatManager>();
  }
}