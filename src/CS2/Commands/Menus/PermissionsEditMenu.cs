using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Permissions;
using Menu;

namespace Commands.Menus;

public class PermissionsEditMenu(IServiceProvider provider, Perm allowedPerms,
  Perm currentPerms)
  : AbstractPagedMenu<Perm?>(provider, NativeSenders.Center, 5) {
  private Perm currentPerm = currentPerms;

  override protected Task<List<Perm?>> GetItems(PlayerWrapper player) {
    var perms = Enum.GetValues<Perm>();
    var list  = new List<Perm?>();
    foreach (var perm in perms) {
      if (perm == Perm.NONE) continue;
      if (allowedPerms.HasFlag(perm)) list.Add(perm);
    }

    list.Add(null); // Save
    return Task.FromResult(list);
  }

  override protected async Task HandleItemSelection(PlayerWrapper player,
    List<Perm?> items, int selectedIndex) {
    var selected = items[selectedIndex];
    if (selected == null) {
      // Save
      var perms = items.Where(p => p != null).Select(p => p.Value).ToArray();
      var perm = Perm.NONE;
      foreach (var p in perms) perm |= p;
      // player.SetData("perms", perm);
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