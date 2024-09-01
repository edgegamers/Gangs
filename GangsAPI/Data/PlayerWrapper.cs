using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;

namespace GangsAPI.Data;

public class PlayerWrapper {
  public readonly string? Name;
  public readonly CCSPlayerController? Player;
  public readonly ulong Steam;
  public AdminData? Data;

  public PlayerWrapper(CCSPlayerController player) {
    Player = player;
    Steam  = player.SteamID;
    Name   = player.PlayerName;

    Data = AdminManager.GetPlayerAdminData(player);
  }

  public PlayerWrapper(ulong steam, string? name) {
    Steam = steam;
    Name  = name;

    Data = new AdminData { Identity = Steam.ToString() };
  }

  private static char USER_CHAR => PermissionCharacters.UserPermissionChar;
  private static char GROUP_CHAR => PermissionCharacters.GroupPermissionChar;

  public PlayerWrapper WithFlags(params string[] flags) {
    var flagMap = new Dictionary<string, HashSet<string>>();
    foreach (var flag in flags) {
      if (!flag.StartsWith(USER_CHAR) && !flag.StartsWith(GROUP_CHAR))
        throw new ArgumentException(
          $"Expected flag ${flag} to start with {USER_CHAR} or {GROUP_CHAR}");

      var slashIndex = flag.IndexOf('/', StringComparison.Ordinal);

      if (slashIndex == -1)
        throw new ArgumentException(
          $"Expected flag ${flag} to contain a / character");

      var domain     = flag[1..slashIndex];
      var permission = flag[(slashIndex + 1)..];

      if (permission.Length == 0)
        throw new ArgumentException(
          $"Expected flag ${flag} to contain a permission after / character");

      if (!flagMap.TryGetValue(domain, out var map)) flagMap[domain] = map = [];

      map.Add(permission);
      flagMap[domain] = map;
    }

    Data = new AdminData {
      Identity = Steam.ToString(), Flags = flagMap, Groups = Data?.Groups ?? []
    };
    return this;
  }

  public bool HasFlags(params string[] flags) {
    if (Data == null) return false;

    foreach (var flag in flags) {
      if (!flag.StartsWith(USER_CHAR))
        throw new ArgumentException(
          $"Expected flag ${flag} to start with {USER_CHAR}");

      var slashIndex = flag.IndexOf('/', StringComparison.Ordinal);

      if (slashIndex == -1)
        throw new ArgumentException(
          $"Expected flag ${flag} to contain a / character");

      var domain     = flag[1..slashIndex];
      var permission = flag[(slashIndex + 1)..];

      if (!Data.Flags.TryGetValue(domain, out var perms)) return false;
      if (perms.Contains("root")) return true;
      if (!perms.Any(p => permission.StartsWith(p))) return false;
    }

    return true;
  }

  public PlayerWrapper WithGroups(params string[] groups) {
    Data = new AdminData {
      Identity = Steam.ToString(),
      Flags    = Data?.Flags ?? new Dictionary<string, HashSet<string>>(),
      Groups   = groups.ToHashSet()
    };
    return this;
  }

  public void PrintToChat(string message) {
    if (Player == null) {
      Console.WriteLine($"{Steam} {Player} received chat: {message}");
      return;
    }

    Player?.PrintToChat(message);
  }

  public void PrintToConsole(string message) {
    if (Player == null) {
      Console.WriteLine($"{Steam} {Player} received console: {message}");
      return;
    }

    Player?.PrintToConsole(message);
  }
}