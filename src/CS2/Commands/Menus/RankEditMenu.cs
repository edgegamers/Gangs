using GangsAPI.Data;
using GangsAPI.Permissions;
using GangsAPI.Services.Menu;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Menus;

public class RankEditMenu(IServiceProvider provider, IGangRank rank)
  : AbstractMenu<string>(provider.GetRequiredService<IMenuManager>(),
    NativeSenders.Chat) {
  override protected Task<List<string>> GetItems(PlayerWrapper player) {
    return Task.FromResult((List<string>) ["Rename", "Delete"]);
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<string> items, int selectedIndex) {
    throw new NotImplementedException();
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    string item) {
    return Task.FromResult($"{index} {item}");
  }
}