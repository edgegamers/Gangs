using GangsAPI.Data;
using GangsAPI.Extensions;
using GangsAPI.Services.Commands;

namespace GangsTest.API.Services.Commands.Command;

public abstract class TestParent {
  protected readonly ICommand Command;
  protected readonly ICommandManager Commands;

  protected readonly PlayerWrapper TestPlayer =
    new(new Random().NextUInt(), "Test Player");

  protected TestParent(ICommandManager commands, ICommand command) {
    Commands = commands;
    Command  = command;
    commands.RegisterCommand(command);
  }
}