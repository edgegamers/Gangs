using System.Diagnostics;
using Commands.gang;
using GangsAPI.Data;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using Microsoft.VisualBasic;

namespace GangsTest.Commands;

public class CreateTests(ICommandManager commands, IGangManager gangMgr)
  : CommandTest(commands, new CreateCommand(gangMgr)) {
  private readonly PlayerWrapper player = new((ulong)new Random().NextInt64(),
    "Test Player");

  [Fact]
  public void Create_TestBase() {
    Assert.Throws<NotImplementedException>(() => {
      Commands.ProcessCommand(player, "create");
    });
  }
}