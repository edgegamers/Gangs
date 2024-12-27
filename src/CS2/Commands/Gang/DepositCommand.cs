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
  public override string[] Usage => ["<amount/all>"];
  public override string Description => "Deposit money into your gang's bank";

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer gangPlayer, CommandInfoWrapper info) {
    if (info.ArgCount != 2) return CommandResult.PRINT_USAGE;

    Debug.Assert(gangPlayer.GangId != null, "gangPlayer.GangId != null");

    var (authorized, required) =
      await Ranks.CheckRank(gangPlayer, Perm.BANK_DEPOSIT);

    if (!authorized) {
      info.ReplySync(Locale.Get(MSG.GENERIC_NOPERM, required.Name));
      return CommandResult.NO_PERMISSION;
    }

    if (!int.TryParse(info[1], out var amount) || amount <= 0) {
      if (!info[1].Equals("all", StringComparison.OrdinalIgnoreCase)) {
        info.ReplySync(Locale.Get(MSG.COMMAND_INVALID_PARAM, info[1],
          "a positive integer"));
        return CommandResult.INVALID_ARGS;
      }

      amount = await Eco.GetBalance(executor, true);
      if (amount <= 0) {
        info.ReplySync(Locale.Get(MSG.COMMAND_BALANCE_NONE));
        return CommandResult.ERROR;
      }
    }

    var remaining =
      await Eco.TryPurchase(executor, amount, true, "deposit", true);
    if (remaining < 0) return CommandResult.ERROR;
    await Eco.Grant(gangPlayer.GangId.Value, amount, true,
      "deposit from " + (executor.Name ?? executor.Steam.ToString()));

    return CommandResult.SUCCESS;
  }
}