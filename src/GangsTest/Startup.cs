using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using GangsTest.TestLocale;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;
using Mock;
using StringLocalizer = GangsTest.TestLocale.StringLocalizer;

namespace GangsTest;

public class Startup {
  public void ConfigureServices(IServiceCollection services) {
    services.AddScoped<IPlayerManager, MockPlayerManager>();
    services.AddScoped<IGangManager, MockGangManager>();
    services.AddScoped<IStatManager, MockStatManager>();
    services.AddScoped<IGangStatManager, MockInstanceStatManager>();
    services.AddScoped<IPlayerStatManager, MockInstanceStatManager>();
    services.AddScoped<ICommandManager, MockCommandManager>();
    services.AddScoped<IMenuManager, MockMenuManager>();
    services.AddScoped<IRankManager, MockRankManager>();
    services
     .TryAddSingleton<IStringLocalizerFactory, LocalFileLocalizerFactory>();
    services.TryAddTransient(typeof(IStringLocalizer), typeof(StringLocalizer));
  }
}