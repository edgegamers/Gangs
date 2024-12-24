using Commands.Gang;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI;
using GangsAPI.Extensions;

namespace GangsTest.Commands.Gang;

public class JoinTests : TestParent {
  public JoinTests(IServiceProvider provider) : base(provider,
    new JoinCommand(provider)) {
    Commands.RegisterCommand(new InviteCommand(provider));
  }

  [Fact]
  public async Task Join_InGang_DoesNotJoin() {
    await Players.CreatePlayer(TestPlayer.Steam, TestPlayer.Name);
    await Gangs.CreateGang("Test Gang", TestPlayer);
    await Gangs.CreateGang("Test Gang 2", new Random().NextULong());
    await Commands.ProcessCommand(TestPlayer, CommandCallingContext.Console,
      "join", "Test Gang 2");
    Assert.Contains(Locale.Get(MSG.ALREADY_IN_GANG), TestPlayer.ConsoleOutput);
  }
}