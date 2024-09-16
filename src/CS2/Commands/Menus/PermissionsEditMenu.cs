using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Menu;
using Menu;
using Microsoft.Extensions.DependencyInjection;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace Commands.Menus;

public class PermissionsEditMenu(IServiceProvider provider, IGang gang,
  Perm allowedPerms, IGangRank currentRank)
  : AbstractPagedMenu<Perm?>(provider, NativeSenders.Center, 6) {
  private Perm currentPerm = currentRank.Permissions;

  private readonly Dictionary<ulong, string> activeTexts = new();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  public override void Start(BasePlugin? plugin, bool hotReload) {
    Server.PrintToConsole("Started permissions editor");
    plugin?.RegisterListener<Listeners.OnTick>(sendText);
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

    list.Add(null); // Save
    return Task.FromResult(list);
  }

  override protected async Task ShowPage(PlayerWrapper player,
    List<Perm?> items, int currentPage, int totalPages) {
    var start = (currentPage - 1) * ItemsPerPage;

    var lineTasks = items.Skip(start)
     .Take(ItemsPerPage)
     .Select(async (p, i) => {
        var index = start + i + 1;
        return await FormatItem(player, index, p);
      });

    var lines = await Task.WhenAll(lineTasks);

    lines = lines.Append("/7 Back, /8 Next, 0. Close").ToArray();

    var text = string.Join("<br>", lines);
    activeTexts[player.Steam] = text;
  }

  override protected async Task HandleItemSelection(PlayerWrapper player,
    List<Perm?> items, int selectedIndex) {
    var selected = items[selectedIndex];
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
    if (item == null) return Task.FromResult("Save");

    var color       = currentPerm.HasFlag(item.Value) ? "#00FF00" : "#FF0000";
    var coloredHtml = $"<font color=\"{color}\">{item.Value.Describe()}</font>";
    var result      = $"{index}. {coloredHtml}";

    if (index == 1)
      result = "Editing permissions for " + currentRank.Name + "<br>" + result;

    return Task.FromResult(result);
  }
}