using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace GangsTest.Commands.ManagerTests;

public class ManagerTests {
  protected readonly ICommand Dummy = new DummyCommand();

  protected readonly PlayerWrapper TestPlayer =
    new((ulong)new Random().NextInt64(), "Test Player");

  private class DummyCommand : ICommand {
    public string Name => "css_dummy";
    public string Description => "Dummy command for testing";
    public string[] RequiredFlags { get; } = [];
    public string[] RequiredGroups { get; } = [];

    public Task<CommandResult> Execute(PlayerWrapper? executor,
      CommandInfoWrapper info) {
      if (info[0] != Name)
        throw new InvalidOperationException("Command name mismatch");

      return Task.FromResult(info[1] == "foobar" ?
        CommandResult.SUCCESS :
        CommandResult.FAILURE);
    }
  }
}