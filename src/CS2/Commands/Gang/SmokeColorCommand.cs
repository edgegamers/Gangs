using System.Diagnostics;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Extensions;
using GangsAPI.Permissions;
using Stats.Perk.Smoke;

namespace Commands.Gang;

public class SmokeColorCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "smokecolor";

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    Debug.Assert(player.GangId != null, "player.GangId != null");
    var gang = await Gangs.GetGang(player.GangId.Value)
      ?? throw new GangNotFoundException(player.GangId.Value);

    var (success, data) =
      await GangStats.GetForGang<SmokePerkData>(gang, SmokeColorPerk.STAT_ID);

    if (!success || data == null) data = new SmokePerkData();

    if (info.ArgCount == 1) {
      var menu = new SmokeColorMenu(Provider, data);
      await Menus.OpenMenu(executor, menu);
      return CommandResult.SUCCESS;
    }

    SmokeColor color;
    var        query = string.Join('_', info.Args.Skip(1)).ToUpper();
    if (!int.TryParse(info[1], out var colorInt) || colorInt < 0) {
      if (!Enum.TryParse(query, out color)) {
        info.ReplySync(
          Locale.Get(MSG.COMMAND_INVALID_PARAM, info[1], "a color"));
        return CommandResult.SUCCESS;
      }
    } else { color = (SmokeColor)colorInt; }

    if (!data.Unlocked.HasFlag(color)) {
      var (canPurchase, minRank) = await Ranks.CheckRank(player,
        Perm.PURCHASE_PERKS);

      if (!canPurchase) {
        info.ReplySync(Locale.Get(MSG.GENERIC_NOPERM_RANK, minRank.Name));
        return CommandResult.SUCCESS;
      }

      var cost = color.GetCost();
      if (await Eco.TryPurchase(executor, cost,
        item: "Smoke Color: " + color.ToString().ToTitleCase()) < 0)
        return CommandResult.SUCCESS;

      data.Unlocked |= color;

      await GangStats.SetForGang(gang, SmokeColorPerk.STAT_ID, data);

      if (GangChat == null) return CommandResult.SUCCESS;

      await GangChat.SendGangChat(player, gang,
        Locale.Get(MSG.PERK_PURCHASED, color.ToString()));
      return CommandResult.SUCCESS;
    }

    if (data.Equipped == color) return CommandResult.SUCCESS;

    var (canManage, required) =
      await Ranks.CheckRank(player, Perm.MANAGE_PERKS);
    if (!canManage) {
      info.ReplySync(Locale.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
      return CommandResult.SUCCESS;
    }

    data.Equipped = color;
    await GangStats.SetForGang(gang, SmokeColorPerk.STAT_ID, data);

    if (GangChat == null) return CommandResult.SUCCESS;

    await GangChat.SendGangChat(player, gang,
      Locale.Get(MSG.GANG_THING_SET, "Smoke Color",
        color.ToString().ToTitleCase()));
    return CommandResult.SUCCESS;
  }
}