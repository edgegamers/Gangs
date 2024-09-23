using System.Diagnostics;
using Commands.Menus;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Extensions;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using Microsoft.Extensions.DependencyInjection;
using Stats.Stat.Gang;

namespace Commands.Gang;

public class DoorPolicyCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  private readonly string doorPolicyId = new DoorPolicyStat().StatId;

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IGangStatManager gangStats =
    provider.GetRequiredService<IGangStatManager>();

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  public override string Name => "doorpolicy";

  public override string[] Usage => ["", "[policy]"];

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    Debug.Assert(player.GangId != null, "player.GangId != null");

    if (info.ArgCount == 2) {
      if (!int.TryParse(info.Args[1], out var selectedIndex)) {
        info.ReplySync(Locale.Get(MSG.COMMAND_INVALID_PARAM, info.Args[1],
          "a number"));
        return CommandResult.SUCCESS;
      }

      var (success, required) =
        await ranks.CheckRank(player, Perm.MANAGE_INVITES);

      if (!success) {
        info.ReplySync(Locale.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
        return CommandResult.SUCCESS;
      }

      var gang = await gangs.GetGang(player.GangId.Value)
        ?? throw new GangNotFoundException(player.GangId.Value);

      var selected =
        (DoorPolicy)(selectedIndex % Enum.GetValues<DoorPolicy>().Length);
      await gangStats.SetForGang(player.GangId.Value, doorPolicyId, selected);

      var gangChat = Provider.GetService<IGangChatPerk>();

      if (gangChat != null)
        await gangChat.SendGangChat(player, gang,
          Locale.Get(MSG.GANG_THING_SET, "Door Policy",
            selected.ToString().ToTitleCase()));

      return CommandResult.SUCCESS;
    }

    var (_, policy) =
      await gangStats.GetForGang<DoorPolicy>(player.GangId.Value, doorPolicyId);

    var menu = new DoorPolicyMenu(Provider, policy);
    await menus.OpenMenu(executor, menu);
    return CommandResult.SUCCESS;
  }
}