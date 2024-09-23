using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Gang;
using GangsAPI.Extensions;
using GangsAPI.Menu;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat.Gang;
using ICommandManager = GangsAPI.Services.Commands.ICommandManager;

namespace Commands.Menus;

public class OutgoingInvitesMenu : AbstractPagedMenu<InvitationEntry?> {
  private readonly DoorPolicy doorPolicy;

  private readonly string doorPolicyId = new DoorPolicyStat().StatId;
  private readonly IGang gang;
  private readonly IGangStatManager gangStats;

  private readonly InvitationStat invitationStat = new();

  private readonly IPlayerManager players;

  public OutgoingInvitesMenu(IServiceProvider provider, IGang gang) : base(
    provider, NativeSenders.Chat) {
    this.gang = gang;
    players   = provider.GetRequiredService<IPlayerManager>();
    gangStats = provider.GetRequiredService<IGangStatManager>();

    var (_, policy) = gangStats.GetForGang<DoorPolicy>(gang, doorPolicyId)
     .GetAwaiter()
     .GetResult();

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

  override protected async Task HandleItemSelection(PlayerWrapper player,
    List<InvitationEntry?> items, int selectedIndex) {
    var entry = items[selectedIndex];

    if (entry == null) {
      await Provider.GetRequiredService<ICommandManager>()
       .ProcessCommand(player, CommandCallingContext.Chat, "css_gang",
          "doorpolicy");
      return;
    }

    var inviterName = await players.GetPlayer(entry.Value.Inviter);
    var invitedName = await players.GetPlayer(entry.Value.Steam);

    await Printer.Invoke(player,
      Localizer.Get(MSG.MENU_FORMAT_INVITATION,
        inviterName?.Name ?? entry.Value.Inviter.ToString(),
        invitedName?.Name ?? entry.Value.Steam.ToString(), entry.Value.Date));
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
    if (item == null)
      return
        $"{ChatColors.DarkRed}{index}. {ChatColors.Grey}Current door policy: {ChatColors.Default}{doorPolicy.ToString().ToTitleCase()}";
    var invited = await players.GetPlayer(item.Value.Steam);
    var inviter = await players.GetPlayer(item.Value.Inviter);

    var invitedName = invited?.Name ?? item.Value.Steam.ToString();
    var inviterName = inviter?.Name ?? item.Value.Inviter.ToString();
    var text        = $"{invitedName} by {inviterName} on {item.Value.Date}";
    return text;
  }
}