using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

public class DepositCommand(IServiceProvider provider) : ICommand {
  public string Name => "deposit";
  public string[] Usage => ["<amount>"];

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  private readonly IStringLocalizer localizer =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    if (info.ArgCount != 2) return CommandResult.PRINT_USAGE;

    var gangPlayer = await players.GetPlayer(executor.Steam);
    if (gangPlayer == null) return CommandResult.SUCCESS;

    if (gangPlayer.GangId == null) {
      info.ReplySync(localizer.Get(MSG.NOT_IN_GANG));
      return CommandResult.SUCCESS;
    }

    var (authorized, required) =
      await ranks.CheckRank(gangPlayer, Perm.BANK_DEPOSIT);

    if (!authorized) {
      info.ReplySync(required == null ?
        localizer.Get(MSG.GENERIC_NOPERM_RANK) :
        localizer.Get(MSG.GENERIC_NOPERM, required.Name));
      return CommandResult.SUCCESS;
    }

    if (!int.TryParse(info[1], out var amount) || amount <= 0) {
      info.ReplySync(localizer.Get(MSG.COMMAND_INVALID_PARAM, info[1],
        "a positive integer"));
      return CommandResult.SUCCESS;
    }

    var remaining = await eco.TryPurchase(gangPlayer, amount, true, "deposit");
    if (remaining >= 0)
      await eco.Grant(gangPlayer.GangId.Value, amount, true, "deposit");

    return CommandResult.SUCCESS;
  }
}