using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace GangsTest.Commands.ManagerTests;

public class ManagerTests {
  protected readonly ICommand Dummy = new DummyCommand();

  protected readonly PlayerWrapper TestPlayer =
    new((ulong)new Random().NextInt64(), "Test Player");

  protected class DummyCommand : ICommand {
    public string Name => "css_dummy";
    public string? Description => "Dummy command for testing";
    public string[] RequiredFlags { get; } = [];
    public string[] RequiredGroups { get; } = [];

    public CommandResult Execute(PlayerWrapper? executor,
      CommandInfoWrapper info) {
      if (info[0] != Name)
        throw new InvalidOperationException("Command name mismatch");

      return info[1] == "foobar" ?
        CommandResult.SUCCESS :
        CommandResult.FAILURE;
    }
  }
}