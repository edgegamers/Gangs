using System.Diagnostics;
using Commands.Menus;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Gang;

public class PermissionCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  public override string Name => "permission";

  public override string[] Aliases => ["permission", "perm", "perms"];

  public override string[] Usage
    => [
      "listranks", "listperms", "<rank>", "grant <rank> <perm>",
      "revoke <rank> <perm>", "set <rank> <int>"
    ];

  private BasePlugin plugin = null!;

  public override void Start(BasePlugin? basePlugin, bool hotReload) {
    this.plugin = basePlugin!;
    base.Start(basePlugin, hotReload);
  }

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    Debug.Assert(player.GangId != null, "player.GangId != null");
    var executorRank = await ranks.GetRank(player)
      ?? throw new GangException("Player has no rank");
    var (allowed, required) = await ranks.CheckRank(player, Perm.MANAGE_RANKS);
    var gang = await gangs.GetGang(player.GangId.Value)
      ?? throw new GangNotFoundException(player.GangId.Value);

    if (!allowed) {
      info.ReplySync(Localizer.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
      return CommandResult.NO_PERMISSION;
    }

    switch (info.ArgCount) {
      case 1: {
        var lowerRanks = (await ranks.GetRanks(player.GangId.Value)).ToList();
        lowerRanks = lowerRanks.Where(r => r.Rank > executorRank.Rank).ToList();

        var menu = new PermissionsRankMenu(Provider, lowerRanks);
        await menus.OpenMenu(executor, menu);
        return CommandResult.SUCCESS;
      }
      case 2 when info.Args[1]
       .Equals("listranks", StringComparison.OrdinalIgnoreCase): {
        var existing = await ranks.GetRanks(player.GangId.Value);
        foreach (var r in existing)
          info.ReplySync(
            $"{ChatColors.LightRed}{r.Rank} {ChatColors.Green}{r.Name}{ChatColors.Grey}: {r.Permissions.GetChatBitfield()}");
        return CommandResult.SUCCESS;
      }
      case 2 when info.Args[1]
       .Equals("listperms", StringComparison.OrdinalIgnoreCase): {
        foreach (var r in Enum.GetValues<Perm>())
          info.ReplySync(
            $"{r.GetChatBitfield()} {ChatColors.Green}{r.ToFriendlyString()}");
        return CommandResult.SUCCESS;
      }
      case 2: {
        var directEdit = await getRank(executor, player, info.Args[1]);
        if (directEdit == null) {
          info.ReplySync(Localizer.Get(MSG.RANK_NOT_FOUND, info.Args[1]));
          return CommandResult.SUCCESS;
        }

        var menu = new PermissionsEditMenu(Provider, plugin, gang,
          executorRank.Permissions, directEdit);
        await menus.OpenMenu(executor, menu);
        break;
      }
    }

    if (info.ArgCount < 4) return CommandResult.PRINT_USAGE;

    var rank = await getRank(executor, player, info.Args[2]);

    if (rank == null) {
      info.ReplySync(Localizer.Get(MSG.RANK_NOT_FOUND, info.Args[2]));
      return CommandResult.SUCCESS;
    }

    if (rank.Rank <= executorRank.Rank) {
      info.ReplySync(Localizer.Get(MSG.RANK_CANNOT_EDIT, rank.Name));
      return CommandResult.SUCCESS;
    }

    Func<Perm, Perm> applicator;

    Perm toSet;
    var  query = string.Join('_', info.Args.Skip(3)).ToUpper();

    if (int.TryParse(info.Args[3], out var permInt)) {
      toSet = (Perm)permInt;
    } else if (!Enum.TryParse(query, true, out toSet)) {
      info.ReplySync(Localizer.Get(MSG.COMMAND_INVALID_PARAM, query,
        "rank or int"));
      return CommandResult.SUCCESS;
    }

    string msg;

    switch (info.Args[1].ToLower()) {
      case "grant":
        applicator = p => p | rank.Permissions;
        msg        = Localizer.Get(MSG.RANK_MODIFY_GRANT, toSet, rank.Name);
        break;
      case "revoke":
        applicator = p => p & ~rank.Permissions;
        msg        = Localizer.Get(MSG.RANK_MODIFY_REVOKE, toSet, rank.Name);
        break;
      case "set":
        applicator = _ => rank.Permissions;
        msg        = Localizer.Get(MSG.RANK_MODIFY_SET, rank.Name, toSet);
        break;
      default:
        return CommandResult.PRINT_USAGE;
    }

    executor.PrintToChat(msg);

    rank.Permissions = applicator(toSet);
    await ranks.UpdateRank(player.GangId.Value, rank);

    var gangChat = Provider.GetService<IGangChatPerk>();
    if (gangChat != null) await gangChat.SendGangChat(player, gang, msg);
    return CommandResult.SUCCESS;
  }

  private async Task<IGangRank?> getRank(PlayerWrapper wrapper,
    IGangPlayer player, string query) {
    Debug.Assert(player.GangId != null, "player.GangId != null");
    if (int.TryParse(query, out var id))
      return await ranks.GetRank(player.GangId.Value, id);
    var existing = await ranks.GetRanks(player.GangId.Value);
    var result = existing.FirstOrDefault(r
      => r.Name.Equals(query, StringComparison.OrdinalIgnoreCase));

    if (result == null)
      wrapper.PrintToChat(Localizer.Get(MSG.RANK_NOT_FOUND, query));

    return result;
  }
}