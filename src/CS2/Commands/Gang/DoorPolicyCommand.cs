using Commands.Menus;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Menu;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Gang;

public class DoorPolicyCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "doorpolicy";

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  override protected Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    var menu = new DoorPolicyMenu(menus,
      info.CallingContext == CommandCallingContext.Chat ?
        NativeSenders.Chat :
        NativeSenders.Console);
    menus.OpenMenu(executor, menu);

    return Task.FromResult(CommandResult.SUCCESS);
  }
}