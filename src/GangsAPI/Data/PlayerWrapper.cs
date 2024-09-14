using System.Diagnostics;
using System.Diagnostics.Contracts;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data.Gang;

namespace GangsAPI.Data;

public class PlayerWrapper {
  private readonly List<string> chatOutput = [],
    consoleOutput = [],
    centerOutput = [];

  public readonly string? Name;
  public readonly CCSPlayerController? Player;
  public readonly ulong Steam;
  public AdminData? Data;
  public CsTeam Team = CsTeam.Spectator;

  public PlayerWrapper(CCSPlayerController player) {
    Player = player;
    Steam  = player.SteamID;
    Name   = player.PlayerName;
    Team   = player.Team;

    Data = AdminManager.GetPlayerAdminData(player);
  }

  public PlayerWrapper(ulong steam, string? name) {
    Steam = steam;
    Name  = name;

    Data = new AdminData { Identity = Steam.ToString() };
  }

  public PlayerWrapper(IGangPlayer player) : this(player.Steam, player.Name) { }

  public IReadOnlyList<string> ChatOutput => chatOutput;
  public IReadOnlyList<string> ConsoleOutput => consoleOutput;
  public IReadOnlyList<string> CenterOutput => centerOutput;

  public bool IsValid => Player == null || Player.IsValid;

  private static char USER_CHAR => PermissionCharacters.UserPermissionChar;
  private static char GROUP_CHAR => PermissionCharacters.GroupPermissionChar;

  public PlayerWrapper WithFlags(params string[] flags) {
    var flagMap = new Dictionary<string, HashSet<string>>();
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

  [Pure]
  public bool HasFlags(params string[] flags) {
    if (Data == null) return false;

    if (Player != null)
      return AdminManager.PlayerHasPermissions(new SteamID(Steam), flags);

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

      if (permission.Length == 0)
        throw new ArgumentException(
          $"Expected flag ${flag} to contain a permission after / character");

      if (!Data.Flags.TryGetValue(domain, out var perms)) return false;
      if (perms.Contains("root")) return true;
      if (!perms.Any(p => permission.StartsWith(p))) return false;
    }

    return true;
  }

  public PlayerWrapper WithGroups(params string[] groups) {
    foreach (var group in groups) {
      if (!group.StartsWith(GROUP_CHAR))
        throw new ArgumentException(
          $"Expected group ${group} to start with {GROUP_CHAR}");

      if (group.Length == 1)
        throw new ArgumentException(
          $"Expected group ${group} to contain a group after {GROUP_CHAR}");
    }

    Data = new AdminData {
      Identity = Steam.ToString(),
      Flags    = Data?.Flags ?? new Dictionary<string, HashSet<string>>(),
      Groups   = groups.ToHashSet()
    };
    return this;
  }

  public void PrintToChat(string message) {
    if (Player == null) {
      Debug.WriteLine($"{Steam} {Name} received chat: {message}");
      chatOutput.Add(message);
      return;
    }

    Server.NextFrame(() => {
      foreach (var s in message.Split('\n')) Player.PrintToChat(s);
    });
  }

  public void PrintToConsole(string message) {
    if (Player == null) {
      Console.WriteLine($"{Steam} {Name} received chat: {message}");
      consoleOutput.Add(message);
      return;
    }

    Server.NextFrame(() => {
      foreach (var s in message.Split('\n')) Player.PrintToConsole(s);
    });
  }

  public void PrintToCenter(string message) {
    if (Player == null) {
      Debug.WriteLine($"{Steam} {Name} received center: {message}");
      centerOutput.Add(message);
      return;
    }

    Server.NextFrame(() => {
      if (Player.IsValid) Player.PrintToCenter(message);
    });
  }
}