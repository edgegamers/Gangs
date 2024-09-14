using Commands;
using Commands.Gang;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using BalanceCommand = Commands.BalanceCommand;

namespace GangsTest.API.Services.Commands.Command;

public class TestData : TheoryData<ICommand> {
  private static readonly IServiceCollection services = new ServiceCollection();

  private readonly ICommand[] behaviors = [
    new BalanceCommand(services.BuildServiceProvider()),
    new global::Commands.Gang.BalanceCommand(services.BuildServiceProvider()),
    new CreateCommand(services.BuildServiceProvider()),
    new DepositCommand(services.BuildServiceProvider()),
    new DisbandCommand(services.BuildServiceProvider()),
    new GangCommand(services.BuildServiceProvider()), new GangDemoteCommand(),
    new GangPromoteCommand(), new GangStatsCommand(),
    new HelpCommand(services.BuildServiceProvider(),
      new Dictionary<string, ICommand>()),
    new InviteCommand(services.BuildServiceProvider()), new KickCommand(),
    new LeaveCommand(), new ManagePermsCommand(),
    new MembersCommand(services.BuildServiceProvider()),
    new PerksCommand(services.BuildServiceProvider()),
    new PurchaseCommand(services.BuildServiceProvider()),
  ];

  static TestData() { services.ConfigureServices(); }

  public TestData() {
    foreach (var behavior in behaviors) {
      behavior.Start();
      Add(behavior);
    }
  }
}