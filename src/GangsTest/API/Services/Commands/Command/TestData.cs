using Commands;
using Commands.Gang;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace GangsTest.API.Services.Commands.Command;

public class TestData : TheoryData<ICommand> {
  private static readonly IServiceCollection services = new ServiceCollection();

  private readonly ICommand[] behaviors = [
    new CreateCommand(services.BuildServiceProvider()), new HelpCommand(),
    new GangCommand(services.BuildServiceProvider()),
    new BalanceCommand(services.BuildServiceProvider()),
    new InviteCommand(services.BuildServiceProvider())
  ];

  static TestData() { services.ConfigureServices(); }

  public TestData() {
    foreach (var behavior in behaviors) {
      behavior.Start();
      Add(behavior);
    }
  }
}