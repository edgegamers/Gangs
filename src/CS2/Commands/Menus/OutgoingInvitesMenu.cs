using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Menu;
using Microsoft.Extensions.DependencyInjection;
using Stats;

namespace Commands.Menus;

public class OutgoingInvitesMenu(IServiceProvider provider, IGang gang)
  : AbstractPagedMenu<InvitationEntry>(provider, NativeSenders.Chat) {
  private readonly InvitationStat invitationStat = new();

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  override protected async Task<List<InvitationEntry>>
    GetItems(PlayerWrapper _) {
    var (success, invites) =
      await gangStats.GetForGang<InvitationData>(gang, invitationStat.StatId);
    if (!success || invites == null) return [];

    return invites.GetEntries();
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<InvitationEntry> items, int selectedIndex) {
    var entry = items[selectedIndex];
    Printer.Invoke(player,
      $"Invitation sent to {entry.Steam} by {entry.Inviter} on {entry.Date}");
    return Task.CompletedTask;
  }

  override protected async Task<string> FormatItem(int index,
    InvitationEntry item) {
    var invited = await players.GetPlayer(item.Steam);
    var inviter = await players.GetPlayer(item.Inviter);

    var invitedName = invited?.Name ?? item.Steam.ToString();
    var inviterName = inviter?.Name ?? item.Inviter.ToString();
    var text        = $"{invitedName} by {inviterName} on {item.Date}";

    if (index == 0)
      text =
        $" \n {ChatColors.DarkRed}!{ChatColors.Red}GANGS {ChatColors.LightRed}Invites {ChatColors.Grey}{gang.Name}\n"
        + text;

    return text;
  }
}