using GangsAPI.Data;
using GangsAPI.Services.Commands;

namespace GangsTest.Commands;

public abstract class CommandTests {
  protected readonly ICommand Command;
  protected readonly ICommandManager Commands;

  protected readonly PlayerWrapper TestPlayer =
    new((ulong)new Random().NextInt64(), "Test Player");

  protected CommandTests(ICommandManager commands, ICommand command) {
    Commands = commands;
    Command  = command;
    commands.RegisterCommand(command);
  }
}