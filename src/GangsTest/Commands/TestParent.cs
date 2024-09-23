using GangsAPI.Services.Commands;

namespace GangsTest.Commands;

public abstract class TestParent : GangTest {
  protected readonly ICommand Command;

  protected TestParent(IServiceProvider provider, ICommand command) :
    base(provider) {
    Command = command;
    Commands.RegisterCommand(command);
    Commands.Start();
  }
}