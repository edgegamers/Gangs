using CounterStrikeSharp.API;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands;

public class CoinflipCommand(IServiceProvider provider) : ICommand {
  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly List<CoinflipRequest> requests = [];

  private readonly IPlayerTargeter targeter =
    provider.GetRequiredService<IPlayerTargeter>();

  public string Name => "css_coinflip";
  public string[] Aliases => ["coinflip", "cf"];
  public string[] Usage => ["accept", "<player> <amount>"];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    if (info.ArgCount == 2
      && info[1].Equals("accept", StringComparison.OrdinalIgnoreCase))
      return await handleAccept(executor, info);

    if (info.ArgCount != 3) return CommandResult.PRINT_USAGE;
    var target = await targeter.GetSingleTarget(info[1], executor, locale);
    if (target == null) return CommandResult.SUCCESS;

    var pending = requests.Where(req => req.Receiver == target.Steam).ToArray();
    if (pending.Length != 0) {
      if (pending.First().Sender == executor.Steam) {
        info.ReplySync(locale.Get(MSG.COMMAND_COINFLIP_COOLDOWN,
          target.Name ?? target.Steam.ToString()));
        return CommandResult.SUCCESS;
      }

      info.ReplySync(locale.Get(MSG.COMMAND_COINFLIP_PENDING,
        target.Name ?? target.Steam.ToString()));
      return CommandResult.SUCCESS;
    }

    if (!int.TryParse(info[2], out var amount) || amount <= 0) {
      info.ReplySync(locale.Get(MSG.COMMAND_INVALID_PARAM, "positive integer"));
      return CommandResult.SUCCESS;
    }

    if (!await eco.CanAfford(executor, amount, true)) {
      info.ReplySync(
        locale.Get(MSG.COMMAND_COINFLIP_INSUFFICIENT_FUNDS, amount));
      return CommandResult.SUCCESS;
    }

    if (!await eco.CanAfford(target, amount, true)) {
      info.ReplySync(locale.Get(MSG.COMMAND_COINFLIP_INSUFFICIENT_FUNDS_OTHER,
        target.Name ?? target.Steam.ToString(), amount));
      return CommandResult.SUCCESS;
    }

    requests.Add(new CoinflipRequest {
      Sender = executor.Steam, Receiver = target.Steam, Amount = amount
    });

    executor.PrintToChat(locale.Get(MSG.COMMAND_COINFLIP_SENT,
      target.Name ?? target.Steam.ToString(), amount));
    target.PrintToChat(locale.Get(MSG.COMMAND_COINFLIP_RECEIVED,
      executor.Name ?? executor.Steam.ToString(), amount));

    await Server.NextFrameAsync(() => {
      Server.RunOnTick(Server.TickCount + 64 * 60, () => {
        var request = requests.FirstOrDefault(req
          => req.Receiver == target.Steam && req.Sender == executor.Steam
          && req.Amount == amount);
        if (request != null) requests.Remove(request);
      });
    });
    return CommandResult.SUCCESS;
  }

  private async Task<CommandResult> handleAccept(PlayerWrapper executor,
    CommandInfoWrapper wrapper) {
    var request =
      requests.FirstOrDefault(req => req.Receiver == executor.Steam);
    if (request == null) {
      wrapper.ReplySync(locale.Get(MSG.COMMAND_COINFLIP_NOPENDING));
      return CommandResult.SUCCESS;
    }

    requests.Remove(request);

    PlayerWrapper? senderWrapper = null;

    await Server.NextFrameAsync(() => {
      var sender = Utilities.GetPlayerFromSteamId(request.Sender);
      if (sender == null) return;

      senderWrapper = new PlayerWrapper(sender);
    });

    if (senderWrapper == null) {
      wrapper.ReplySync(locale.Get(MSG.COMMAND_COINFLIP_INVALID));
      return CommandResult.SUCCESS;
    }

    // Make sure they still have the money
    if (!await eco.CanAfford(executor, request.Amount, true)) {
      wrapper.ReplySync(locale.Get(MSG.COMMAND_COINFLIP_INSUFFICIENT_FUNDS,
        request.Amount));
      return CommandResult.SUCCESS;
    }

    if (!await eco.CanAfford(executor, request.Amount, true)) {
      wrapper.ReplySync(locale.Get(
        MSG.COMMAND_COINFLIP_INSUFFICIENT_FUNDS_OTHER,
        executor.Name ?? executor.Steam.ToString(), request.Amount));
      return CommandResult.SUCCESS;
    }

    var senderWon = new Random().Next(0, 2) == 0;
    var winner    = senderWon ? senderWrapper : executor;
    var loser     = senderWon ? executor : senderWrapper;

    await eco.Grant(winner, request.Amount, reason: "Coinflip");
    await eco.Grant(loser, -request.Amount, reason: "Coinflip");
    var msg = locale.Get(MSG.COMMAND_COINFLIP_RESULT,
      winner.Name ?? winner.Steam.ToString(),
      loser.Name ?? loser.Steam.ToString(), request.Amount);

    var targets = new List<PlayerWrapper> { winner, loser };
    if (request.Amount >= 10000)
      await Server.NextFrameAsync(() => {
        targets = targets.Union(Utilities.GetPlayers()
           .Select(p => new PlayerWrapper(p))
           .Where(p => p.Steam != winner.Steam && p.Steam != loser.Steam)
           .ToList())
         .ToList();
      });

    foreach (var target in targets) target.PrintToChat(msg);

    return CommandResult.SUCCESS;
  }

  private class CoinflipRequest {
    public ulong Sender { get; init; }
    public ulong Receiver { get; init; }
    public int Amount { get; init; }
  }
}