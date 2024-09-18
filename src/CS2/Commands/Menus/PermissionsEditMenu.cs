using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Menu;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Menus;

public class PermissionsEditMenu : AbstractPagedMenu<Perm?> {
  private readonly Dictionary<ulong, string> activeTexts = new();
  private readonly Perm allowedPerms;
  private readonly ICommandManager commands;
  private readonly IGangRank currentRank;
  private readonly IGang gang;
  private readonly BasePlugin plugin;

  private readonly IRankManager ranks;

  private Perm currentPerm;

  public PermissionsEditMenu(IServiceProvider provider, BasePlugin plugin,
    IGang gang, Perm allowedPerms, IGangRank currentRank) : base(provider,
    NativeSenders.Center, 6) {
    ranks             = provider.GetRequiredService<IRankManager>();
    commands          = provider.GetRequiredService<ICommandManager>();
    this.gang         = gang;
    this.allowedPerms = allowedPerms;
    this.currentRank  = currentRank;
    this.plugin       = plugin;
    currentPerm       = currentRank.Permissions;

    Server.NextFrame(() => plugin.RegisterListener<Listeners.OnTick>(sendText));
  }

  public override void Dispose() {
    plugin.RemoveListener<Listeners.OnTick>(sendText);
  }

  public override Task Close(PlayerWrapper player) {
    activeTexts.Remove(player.Steam);
    Server.NextFrame(() => plugin.RemoveListener<Listeners.OnTick>(sendText));
    return Task.CompletedTask;
  }

  private void sendText() {
    foreach (var (steam, text) in activeTexts) {
      var player = Utilities.GetPlayerFromSteamId(steam);
      if (player == null || !player.IsValid) continue;
      Printer(new PlayerWrapper(player), text);
    }
  }

  override protected Task<List<Perm?>> GetItems(PlayerWrapper player) {
    var perms = Enum.GetValues<Perm>();
    var list = perms.Where(perm => perm != Perm.NONE && perm != Perm.OWNER)
     .Where(perm => allowedPerms.HasFlag(perm))
     .Select(perm => (Perm?)perm)
     .ToList();
    list.Add(null);
    return Task.FromResult(list);
  }

  override protected async Task ShowPage(PlayerWrapper player,
    List<Perm?> items, int currentPage, int totalPages) {
    CurrentPages[player.Steam] = currentPage;
    var lineTasks =
      items.Select(async (p, i) => await FormatItem(player, i + 1, p));

    var lines = await Task.WhenAll(lineTasks);

    var text = string.Join("<br>", lines);
    activeTexts[player.Steam] = text;
  }

  public override async Task AcceptInput(PlayerWrapper player, int input) {
    var start      = (GetCurrentPage(player) - 1) * ItemsPerPage;
    var allItems   = await GetItems(player);
    var pageItems  = allItems.Skip(start).Take(ItemsPerPage).ToList();
    var totalPages = (allItems.Count + ItemsPerPage - 1) / ItemsPerPage;

    var currentPage = GetCurrentPage(player);
    switch (input) {
      case 0:
        await Close(player);
        break;
      // Handle page navigation
      case 7 when HasPreviousPage(player): {
        pageItems = allItems.Skip((currentPage - 2) * ItemsPerPage)
         .Take(ItemsPerPage)
         .ToList();
        await ShowPage(player, pageItems, currentPage - 1, totalPages);
        break;
      }
      case 8 when HasNextPage(player): {
        pageItems = allItems.Skip(currentPage * ItemsPerPage)
         .Take(ItemsPerPage)
         .ToList();
        await ShowPage(player, pageItems, currentPage + 1, totalPages);
        break;
      }
      case 9:
        await Close(player);
        await commands.ProcessCommand(player, CommandCallingContext.Chat,
          "css_gang", "permission", "set", currentRank.Rank.ToString(),
          ((int)currentPerm).ToString());
        break;
      default:
        await HandleItemSelection(player, pageItems, input - 1);
        break;
    }
  }

  override protected async Task HandleItemSelection(PlayerWrapper player,
    List<Perm?> items, int selectedIndex) {
    var selected = items[selectedIndex];
    if (selected == null) {
      currentRank.Permissions = currentPerm;
      await ranks.UpdateRank(gang.GangId, currentRank);
      return;
    }

    if (currentPerm.HasFlag(selected.Value))
      currentPerm &= ~selected.Value;
    else
      currentPerm |= selected.Value;

    await ShowPage(player, items, GetCurrentPage(player),
      (items.Count + ItemsPerPage - 1) / ItemsPerPage);
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    Perm? item) {
    var hasNextPage = HasNextPage(player);
    var hasPrevPage = HasPreviousPage(player);

    var footer = "/9 Save";

    if (hasPrevPage) footer =  "&lt;- /7 | " + footer;
    if (hasNextPage) footer += " | /8 -&gt;";

    if (item == null) return Task.FromResult(footer);

    var color       = currentPerm.HasFlag(item.Value) ? "#00FF00" : "#FF0000";
    var coloredHtml = $"<font color=\"{color}\">{item.Value.Describe()}</font>";
    var result      = $"{index}. {coloredHtml}";

    if (index == 1) result = currentRank.Name + " perms<br>" + result;

    if (index == ItemsPerPage) result += "<br>" + footer;
    return Task.FromResult(result);
  }
}