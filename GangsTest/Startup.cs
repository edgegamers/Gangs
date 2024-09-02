using System.Text;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualBasic.CompilerServices;
using Mock;
using Xunit.Abstractions;
using Xunit.DependencyInjection.Logging;

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