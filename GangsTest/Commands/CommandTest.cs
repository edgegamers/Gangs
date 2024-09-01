using GangsAPI.Data;
using GangsAPI.Services.Commands;

namespace GangsTest.Commands;

public abstract class CommandTest {
  protected readonly ICommandManager Commands;
  protected readonly ICommand Command;

  protected readonly PlayerWrapper TestPlayer =
    new((ulong)new Random().NextInt64(), "Test Player");

  protected CommandTest(ICommandManager commands, ICommand command) {
    Commands = commands;
    Command  = command;
    commands.RegisterCommand(command);
  }
}