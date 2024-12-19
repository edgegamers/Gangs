using System.Drawing;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Timer = CounterStrikeSharp.API.Modules.Timers.Timer;

namespace Stats.Perk.Smoke;

public class SmokeListener(IServiceProvider provider) : IPluginBehavior {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly Dictionary<ulong, Color?> smokeColors = new();
  private readonly Dictionary<IntPtr, Timer> smokeTimers = new();

  private BasePlugin plugin = null!;

  private readonly Random rng = new();

  public void Start(BasePlugin? plugin, bool hotReload) {
    if (plugin == null) return;
    this.plugin = plugin;
    this.plugin.RegisterListener<Listeners.OnEntitySpawned>(OnEntitySpawned);
  }

  private void OnEntitySpawned(CEntityInstance entity) {
    if (entity.DesignerName != "smokegrenade_projectile") return;

    CSmokeGrenadeProjectile proj = new(entity.Handle);
    if (proj.Handle == IntPtr.Zero || !proj.IsValid) return;

    var timer = plugin.AddTimer(1, () => {
      var thrower = proj.Thrower.Value?.Controller.Value;
      if (thrower == null || !thrower.IsValid) { goto kill; }

      if (!smokeColors.TryGetValue(thrower.SteamID, out var color)) {
        goto kill;
      }

      if (!proj.IsValid) goto kill;

      color ??= Color.FromArgb(rng.Next(0, 256), rng.Next(0, 256),
        rng.Next(0, 256));

      proj.SmokeColor.X = color.Value.R;
      proj.SmokeColor.Y = color.Value.G;
      proj.SmokeColor.Z = color.Value.B;

      return;

    kill:
      smokeTimers[proj.Handle].Kill();
      smokeTimers.Remove(proj.Handle);
    });

    smokeTimers[proj.Handle] = timer;
  }

  [GameEventHandler]
  public HookResult OnRoundStart(EventRoundStart ev, GameEventInfo info) {
    var wrapped = Utilities.GetPlayers()
     .Where(p => !p.IsBot)
     .Select(p => new PlayerWrapper(p))
     .ToList();

    smokeColors.Clear();
    Task.Run(async () => await fetchSmokeColors(wrapped));
    return HookResult.Continue;
  }

  private async Task fetchSmokeColors(List<PlayerWrapper> wrappers) {
    Dictionary<int, Color?> cachedGangs = new();
    foreach (var wrapper in wrappers) {
      var player = await players.GetPlayer(wrapper.Steam);
      if (player?.GangId == null) continue;

      if (!cachedGangs.TryGetValue(player.GangId.Value, out var color)) {
        var gang = await gangs.GetGang(player.GangId.Value);
        if (gang == null) continue;
        var (success, data) =
          await gangStats.GetForGang<SmokePerkData>(gang,
            SmokeColorPerk.STAT_ID);
        if (!success || data == null) {
          cachedGangs.Add(player.GangId.Value, null);
          continue;
        }

        if (data.Equipped == SmokeColor.RAINBOW) {
          color                      = null;
          smokeColors[wrapper.Steam] = null;
        } else
          color = data.Equipped.GetColor() ?? data.Unlocked.PickRandom();

        cachedGangs.Add(player.GangId.Value, color);
      }

      if (color != null) smokeColors[wrapper.Steam] = color.Value;
    }
  }
}