using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Menu;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat.Gang;
using ICommandManager = GangsAPI.Services.Commands.ICommandManager;

namespace Commands.Menus;

public class OutgoingInvitesMenu : AbstractPagedMenu<InvitationEntry?> {
  private readonly IGangStatManager gangStats;

  private readonly InvitationStat invitationStat = new();

  private readonly string doorPolicyId = new DoorPolicyStat().StatId;

  private readonly IPlayerManager players;
  private readonly IGang gang;

  private readonly DoorPolicy doorPolicy;

  public OutgoingInvitesMenu(IServiceProvider provider, IGang gang) : base(
    provider, NativeSenders.Chat) {
    this.gang = gang;
    players   = provider.GetRequiredService<IPlayerManager>();
    gangStats = provider.GetRequiredService<IGangStatManager>();

    var (_, policy) = gangStats.GetForGang<DoorPolicy>(gang, doorPolicyId)
     .GetAwaiter()
     .GetResult();
    Server.NextFrame(() => {
      Server.PrintToChatAll($"Door policy: {policy}");
    });

    doorPolicy = policy;
  }

  override protected async Task<List<InvitationEntry?>> GetItems(
    PlayerWrapper _) {
    var results = new List<InvitationEntry?> { null };
    var (success, invites) =
      await gangStats.GetForGang<InvitationData>(gang, invitationStat.StatId);
    if (!success || invites == null) return results;

    var entries = invites.GetEntries();
    results.AddRange(entries.Select(entry => (InvitationEntry?)entry));
    return results;
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<InvitationEntry?> items, int selectedIndex) {
    var entry = items[selectedIndex];

    if (entry == null) {
      Provider.GetRequiredService<ICommandManager>()
       .ProcessCommand(player, "css_gang", "doorpolicy");
      return Task.CompletedTask;
    }

    Printer.Invoke(player,
      $"Invitation sent to {entry.Value.Steam} by {entry.Value.Inviter} on {entry.Value.Date}");
    return Task.CompletedTask;
  }

  override protected Task ShowPage(PlayerWrapper player,
    List<InvitationEntry?> items, int currentPage, int totalPages) {
    if (currentPage == 0)
      player.PrintToChat(
        $"{ChatColors.DarkRed}!{ChatColors.Red}GANGS {ChatColors.LightRed}Invites {ChatColors.Grey}{gang.Name}");
    return base.ShowPage(player, items, currentPage, totalPages);
  }

  override protected async Task<string> FormatItem(PlayerWrapper player,
    int index, InvitationEntry? item) {
    if (item == null) return "Current door policy: " + doorPolicy;
    var invited = await players.GetPlayer(item.Value.Steam);
    var inviter = await players.GetPlayer(item.Value.Inviter);

    var invitedName = invited?.Name ?? item.Value.Steam.ToString();
    var inviterName = inviter?.Name ?? item.Value.Inviter.ToString();
    var text        = $"{invitedName} by {inviterName} on {item.Value.Date}";
    return text;
  }
}