using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;
using GangsAPI.Services;
using Menu;
using Microsoft.Extensions.DependencyInjection;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace Commands.Menus;

public class PermissionsEditMenu(IServiceProvider provider, IGang gang,
  Perm allowedPerms, IGangRank currentRank)
  : AbstractPagedMenu<Perm?>(provider, NativeSenders.Center, 5) {
  private Perm currentPerm = currentRank.Permissions;

  private readonly Dictionary<PlayerWrapper, Timer> timers = new();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  public override Task Close(PlayerWrapper player) {
    if (timers.TryGetValue(player, out var timer)) timer.Kill();
    return Task.CompletedTask;
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

  override protected Task ShowPage(PlayerWrapper player, List<Perm?> items,
    int currentPage, int totalPages) {
    var start = (currentPage - 1) * ItemsPerPage;

    var lines = items.Skip(start)
     .Take(ItemsPerPage)
     .Select((p, i) => {
        var index = start + i + 1;
        return FormatItem(player, index, p);
      });

    var text = string.Join("\n", lines);

    timers[player] = new Timer(0.1f, () => Printer(player, text),
      TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
    return Task.CompletedTask;
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

    await Open(player);
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    Perm? item) {
    if (item == null) return Task.FromResult("Save");

    var color = currentPerm.HasFlag(item.Value) ?
      ChatColors.Green :
      ChatColors.Red;

    return Task.FromResult($"{index}. {color}{item.Value.Describe()}");
  }
}