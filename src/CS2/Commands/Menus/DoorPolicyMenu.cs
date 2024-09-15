using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Menu;
using Menu;
using Microsoft.Extensions.DependencyInjection;
using ICommandManager = GangsAPI.Services.Commands.ICommandManager;

namespace Commands.Menus;

public class DoorPolicyMenu(IServiceProvider provider, DoorPolicy active)
  : AbstractMenu<DoorPolicy>(provider.GetRequiredService<IMenuManager>(),
    NativeSenders.Chat) {
  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

  override protected Task<List<DoorPolicy>> GetItems(PlayerWrapper player) {
    return Task.FromResult(Enum.GetValues<DoorPolicy>().ToList());
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<DoorPolicy> items, int selectedIndex) {
    commands.ProcessCommand(player, "css_gang", "doorpolicy",
      (selectedIndex - 1).ToString());
    return Task.CompletedTask;
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    DoorPolicy item) {
    return Task.FromResult(item == active ?
      $"*{index + 1}. {item}" :
      $"{index + 1}. {item}");
  }
}