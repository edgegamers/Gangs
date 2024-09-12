using GangsAPI.Data;
using GangsAPI.Perks;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Stats.Perk;

public class BasicPerkMenu(IServiceProvider provider, IPerk perk)
  : AbstractMenu<string>(provider.GetRequiredService<IMenuManager>(),
    NativeSenders.Chat) {
  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  public override async Task Open(PlayerWrapper player) {
    var items = await GetItems(player);
    await Show(player, items);
  }

  public override async Task AcceptInput(PlayerWrapper player, int input) {
    await HandleItemSelection(player, await GetItems(player), input);
  }

  override protected async Task<List<string?>> GetItems(PlayerWrapper player) {
    var gangPlayer = await players.GetPlayer(player.Steam);

    if (gangPlayer?.GangId == null || gangPlayer.GangRank == null) return [];

    var cost  = await perk.GetCost(gangPlayer);
    var items = new List<string?> { $"Gang Perk: {perk.Name} ({cost})" };
    if (perk.Description != null) items.Add(perk.Description);
    items.Add("1. Purchase");
    items.Add("2. Cancel");
    return items;
  }

  override protected async Task HandleItemSelection(PlayerWrapper player,
    List<string?> items, int selectedIndex) {
    if (selectedIndex == 1) {
      var gangPlayer = await players.GetPlayer(player.Steam);
      if (gangPlayer?.GangId == null || gangPlayer.GangRank == null) return;
      await perk.OnPurchase(gangPlayer);
    }
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    string? item) {
    return Task.FromResult(item ?? "");
  }
}