using System.Diagnostics;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Permissions;

namespace Commands.Gang;

public class DepositCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "deposit";
  public override string[] Usage => ["<amount>"];

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer gangPlayer, CommandInfoWrapper info) {
    if (info.ArgCount != 2) return CommandResult.PRINT_USAGE;

    Debug.Assert(gangPlayer.GangId != null, "gangPlayer.GangId != null");

    var (authorized, required) =
      await Ranks.CheckRank(gangPlayer, Perm.BANK_DEPOSIT);

    if (!authorized) {
      info.ReplySync(Localizer.Get(MSG.GENERIC_NOPERM, required.Name));
      return CommandResult.SUCCESS;
    }

    if (!int.TryParse(info[1], out var amount) || amount <= 0) {
      info.ReplySync(Localizer.Get(MSG.COMMAND_INVALID_PARAM, info[1],
        "a positive integer"));
      return CommandResult.SUCCESS;
    }

    var remaining =
      await Eco.TryPurchase(executor, amount, true, "deposit", true);
    if (remaining >= 0) {
      await Eco.Grant(gangPlayer.GangId.Value, amount, true, "deposit");
    }

    return CommandResult.SUCCESS;
  }
}