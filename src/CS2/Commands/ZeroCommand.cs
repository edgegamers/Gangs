using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands;

public class ZeroCommand(IServiceProvider provider) : ICommand {
  public string Name => "css_0";

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    var menu = provider.GetService<IMenuManager>();
    if (menu == null) return Task.FromResult(CommandResult.SUCCESS);

    if (executor == null) return Task.FromResult(CommandResult.PLAYER_ONLY);

    Task.Run(() => menu.AcceptInput(executor, 0));
    return Task.FromResult(CommandResult.SUCCESS);
  }
}