using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;

namespace GangsAPI.Data;

public class PlayerWrapper {
  public readonly string? Name;
  public readonly CCSPlayerController? Player;
  public readonly ulong Steam;
  public AdminData? data;

  public PlayerWrapper(CCSPlayerController player) {
    Player = player;
    Steam  = player.SteamID;
    Name   = player.PlayerName;

    data = AdminManager.GetPlayerAdminData(player);
  }

  public PlayerWrapper(ulong steam, string? name) {
    Steam = steam;
    Name  = name;

    data = new AdminData { Identity = Steam.ToString() };
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

      var domain     = flag[..slashIndex];
      var permission = flag[(slashIndex + 1)..];

      if (!flagMap.TryGetValue(domain, out var map)) flagMap[domain] = map = [];

      map.Add(permission);
      flagMap[domain] = map;
    }

    data = new AdminData {
      Identity = Steam.ToString(), Flags = flagMap, Groups = data?.Groups ?? []
    };
    return this;
  }

  public PlayerWrapper WithGroups(params string[] groups) {
    data = new AdminData {
      Identity = Steam.ToString(),
      Flags    = data?.Flags ?? new Dictionary<string, HashSet<string>>(),
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