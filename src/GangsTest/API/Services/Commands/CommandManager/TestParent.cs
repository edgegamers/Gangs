using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;

namespace GangsTest.API.Services.Commands.CommandManager;

public class TestParent {
  protected readonly ICommand Dummy = new DummyCommand();

  protected readonly PlayerWrapper TestPlayer =
    new((ulong)new Random().NextInt64(), "Test Player");

  protected class DummyCommand : ICommand {
    public virtual string Name => "css_dummy";
    public virtual string Description => "Dummy command for testing";
    public virtual string[] RequiredFlags { get; } = [];
    public virtual string[] RequiredGroups { get; } = [];
    public virtual string[] Aliases => [Name];

    public virtual Task<CommandResult> Execute(PlayerWrapper? executor,
      CommandInfoWrapper info) {
      if (info[0] != Name)
        throw new InvalidOperationException("Command name mismatch");

      return Task.FromResult(info[1] == "foobar" ?
        CommandResult.SUCCESS :
        CommandResult.ERROR);
    }
  }
}