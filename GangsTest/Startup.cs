using GangsAPI.Services;
using GangsImpl.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace GangsTest;

public class Startup {
  public void ConfigureServices(IServiceCollection services) {
    services.AddScoped<IPlayerManager, MockPlayerManager>();
    services.AddScoped<IGangManager, MockGangManager>();
    services.AddScoped<IStatManager, MockStatManager>();
    services.AddScoped<IGangStatManager, MockInstanceStatManager>();
    services.AddScoped<IPlayerStatManager, MockInstanceStatManager>();
  }
}