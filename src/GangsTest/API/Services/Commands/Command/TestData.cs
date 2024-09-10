using Commands;
using Commands.Gang;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using GangsTest.TestLocale;
using Mock;

namespace GangsTest.API.Services.Commands.Command;

public class TestData : TheoryData<ICommand> {
  private static readonly IPlayerManager playerMgr = new MockPlayerManager();
  private static readonly IMenuManager menuMgr = new MockMenuManager();
  private static readonly IRankManager rankMgr = new MockRankManager(playerMgr);

  private static readonly IGangStatManager gangStatMgr =
    new MockInstanceStatManager();

  private static readonly IPlayerStatManager statMgr =
    new MockInstanceStatManager();


  private static readonly IGangManager manager =
    new MockGangManager(playerMgr, rankMgr);

  private readonly ICommand[] behaviors = [
    new CreateCommand(manager, StringLocalizer.Instance), new HelpCommand(),
    new GangCommand(manager, playerMgr, menuMgr, rankMgr, gangStatMgr,
      StringLocalizer.Instance),
    new BalanceCommand(statMgr, StringLocalizer.Instance),
    new InviteCommand(manager, playerMgr, rankMgr, gangStatMgr,
      StringLocalizer.Instance)
  ];

  public TestData() {
    foreach (var behavior in behaviors) {
      behavior.Start();
      Add(behavior);
    }
  }
}