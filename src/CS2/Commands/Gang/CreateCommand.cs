using System.Text.RegularExpressions;
using CounterStrikeSharp.API;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

/// <summary>
///   Allows players to create a new gang
/// </summary>
/// <param name="provider"></param>
public class CreateCommand(IServiceProvider provider) : ICommand {
  private const int CREATION_COST = 2000;

  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  public string Name => "create";
  public string Description => "Create a gang";
  public string[] Usage => ["[name]"];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;
    if (info.ArgCount < 2) return CommandResult.PRINT_USAGE;

    var name = string.Join(' ', info.Args.Skip(1)).Trim();

    if (await gangs.GetGang(executor.Steam) != null) {
      info.ReplySync(locale.Get(MSG.ALREADY_IN_GANG));
      return CommandResult.ERROR;
    }

    if (name.Length is < 1 or > 16) {
      info.ReplySync(locale.Get(MSG.COMMAND_GANG_CREATE_INVALID));
      return CommandResult.ERROR;
    }

    // Regex check for invalid characters
    if (!Regex.IsMatch(name, @"^[a-zA-Z0-9!@#$%^&*()-=_+?.{}\[\]' ]+$")) {
      info.ReplySync(locale.Get(MSG.COMMAND_GANG_CREATE_INVALID));
      return CommandResult.ERROR;
    }

    var first = (await gangs.GetGangs()).FirstOrDefault(g
      => g.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    if (first != null) {
      info.ReplySync(locale.Get(MSG.COMMAND_GANG_CREATE_ALREADY_EXISTS,
        first.Name));
      return CommandResult.ERROR;
    }

    if (await eco.TryPurchase(executor, CREATION_COST, item: "Gang Creation")
      < 0)
      return CommandResult.SUCCESS;

    var newGang = await gangs.CreateGang(name, executor.Steam);
    if (newGang == null) {
      info.ReplySync(locale.Get(MSG.GENERIC_ERROR_INFO,
        "Failed to create a gang"));
      return CommandResult.ERROR;
    }

    if (newGang.GangId == 0) {
      info.ReplySync(locale.Get(MSG.GENERIC_ERROR_INFO, "GangID returned 0"));
      return CommandResult.ERROR;
    }

    info.ReplySync(locale.Get(MSG.COMMAND_GANG_CREATE_SUCCESS, name,
      newGang.GangId));
    var msg = locale.Get(MSG.GANG_CREATED,
      executor.Name ?? executor.Steam.ToString(), name);

    if (executor.Player != null)
      await Server.NextFrameAsync(() => Server.PrintToChatAll(msg));
    return CommandResult.SUCCESS;
  }
}