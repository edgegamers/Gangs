using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Menu;
using Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Menus;

public class PermissionsEditMenu : AbstractPagedMenu<Perm?> {
  public PermissionsEditMenu(IServiceProvider provider, BasePlugin plugin,
    IGang gang, Perm allowedPerms, IGangRank currentRank) : base(provider,
    NativeSenders.Center, 6) {
    ranks             = provider.GetRequiredService<IRankManager>();
    menus             = provider.GetRequiredService<IMenuManager>();
    commands          = provider.GetRequiredService<ICommandManager>();
    this.gang         = gang;
    this.allowedPerms = allowedPerms;
    this.currentRank  = currentRank;
    this.plugin       = plugin;
    currentPerm       = currentRank.Permissions;

    Server.NextFrame(() => plugin.RegisterListener<Listeners.OnTick>(sendText));
  }

  private Perm currentPerm;
  private readonly Dictionary<ulong, string> activeTexts = new();

  private readonly IRankManager ranks;
  private readonly IMenuManager menus;
  private readonly IGang gang;
  private readonly Perm allowedPerms;
  private readonly IGangRank currentRank;
  private readonly ICommandManager commands;
  private readonly BasePlugin plugin;

  public override void Dispose() {
    plugin.RemoveListener<Listeners.OnTick>(sendText);
  }

  public override Task Close(PlayerWrapper player) {
    activeTexts.Remove(player.Steam);
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
    var list = perms.Where(perm => perm != Perm.NONE)
     .Where(perm => allowedPerms.HasFlag(perm))
     .Select(perm => (Perm?)perm)
     .ToList();
    return Task.FromResult(list);
  }

  override protected async Task ShowPage(PlayerWrapper player,
    List<Perm?> items, int currentPage, int totalPages) {
    var start = (currentPage - 1) * ItemsPerPage;

    var lineTasks = items.Skip(start)
     .Take(ItemsPerPage)
     .Select(async (p, i) => await FormatItem(player, i - start + 1, p));

    var lines = await Task.WhenAll(lineTasks);

    lines = lines.Append("/7 Back, /8 Next, /9 Save").ToArray();
    lines = lines.Prepend("/0 Cancel").ToArray();

    var text = string.Join("<br>", lines);
    activeTexts[player.Steam] = text;
  }

  public async override Task AcceptInput(PlayerWrapper player, int input) {
    var items      = await GetItems(player);
    var totalPages = (items.Count + ItemsPerPage - 1) / ItemsPerPage;

    switch (input) {
      case 0:
        await Close(player);
        break;
      // Handle page navigation
      case 8 when HasNextPage(player): {
        var currentPage = GetCurrentPage(player);
        await ShowPage(player, items, currentPage + 1, totalPages);
        break;
      }
      case 7 when HasPreviousPage(player): {
        var currentPage = GetCurrentPage(player);
        await ShowPage(player, items, currentPage - 1, totalPages);
        break;
      }
      case 9:
        await commands.ProcessCommand(player, "css_gang", "permission", "set",
          currentRank.Rank.ToString(), ((int)currentPerm).ToString());
        await Close(player);
        break;
      default:
        await HandleItemSelection(player, items, input - 1);
        break;
    }
  }

  override protected async Task HandleItemSelection(PlayerWrapper player,
    List<Perm?> items, int selectedIndex) {
    var selected = items[selectedIndex];
    player.PrintToChat($"Selected {selected}");
    if (selected == null) {
      currentRank.Permissions = currentPerm;
      await ranks.UpdateRank(gang.GangId, currentRank);
      return;
    }

    if (currentPerm.HasFlag(selected.Value)) {
      currentPerm &= ~selected.Value;
    } else { currentPerm |= selected.Value; }

    await menus.OpenMenu(player, this);
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    Perm? item) {
    if (item == null) return Task.FromResult("9. Save");

    var color       = currentPerm.HasFlag(item.Value) ? "#00FF00" : "#FF0000";
    var coloredHtml = $"<font color=\"{color}\">{item.Value.Describe()}</font>";
    var result      = $"{index}. {coloredHtml}";

    if (index == 1)
      result = "Editing permissions for " + currentRank.Name + "<br>" + result;

    return Task.FromResult(result);
  }
}