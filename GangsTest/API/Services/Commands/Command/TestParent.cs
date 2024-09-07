using GangsAPI.Data;
using GangsAPI.Services.Commands;

namespace GangsTest.API.Services.Commands.Command;

public abstract class TestParent {
  protected readonly ICommand Command;
  protected readonly ICommandManager Commands;

  protected readonly PlayerWrapper TestPlayer =
    new((ulong)new Random().NextInt64(), "Test Player");

  protected TestParent(ICommandManager commands, ICommand command) {
    Commands = commands;
    Command  = command;
    commands.RegisterCommand(command);
  }
}