using System.Diagnostics;
using Commands.gang;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using Microsoft.VisualBasic;

namespace GangsTest.Commands;

public class CreateTests(ICommandManager commands, IGangManager gangMgr)
  : CommandTests(commands, new CreateCommand(gangMgr)) {
  private readonly PlayerWrapper player = new((ulong)new Random().NextInt64(),
    "Test Player");

  [Fact]
  public async Task Create_TestNonPlayer() {
    Assert.Equal(CommandResult.PLAYER_ONLY,
      await Commands.ProcessCommand(null, "create"));
  }

  [Fact]
  public async Task Create_TestNoName() {
    Assert.Equal(CommandResult.FAILURE,
      await Commands.ProcessCommand(player, "create"));
    Assert.Contains("Please provide a name for the gang", player.ConsoleOutput);
  }
}