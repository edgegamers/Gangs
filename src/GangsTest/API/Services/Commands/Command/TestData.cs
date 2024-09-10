using Commands;
using Commands.Gang;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using GangsAPI.Services.Server;
using GangsTest.TestLocale;
using Mock;

namespace GangsTest.API.Services.Commands.Command;

public class TestData : TheoryData<ICommand> {
  private static readonly IPlayerManager players = new MockPlayerManager();
  private static readonly IMenuManager menus = new MockMenuManager();
  private static readonly IRankManager ranks = new MockRankManager(players);

  private static readonly ITargeter targeter =
    new MockTargeter(new MockServerProvider());

  private static readonly IGangStatManager gangStats =
    new MockInstanceStatManager();

  private static readonly IPlayerStatManager stats =
    new MockInstanceStatManager();


  private static readonly IGangManager manager =
    new MockGangManager(players, ranks);

  private readonly ICommand[] behaviors = [
    new CreateCommand(manager, StringLocalizer.Instance), new HelpCommand(),
    new GangCommand(manager, players, menus, ranks, gangStats, targeter,
      StringLocalizer.Instance),
    new BalanceCommand(stats, StringLocalizer.Instance),
    new InviteCommand(manager, players, ranks, gangStats, targeter,
      StringLocalizer.Instance)
  ];

  public TestData() {
    foreach (var behavior in behaviors) {
      behavior.Start();
      Add(behavior);
    }
  }
}