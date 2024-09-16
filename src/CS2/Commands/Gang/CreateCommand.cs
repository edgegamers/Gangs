using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

// create [name]
public class CreateCommand(IServiceProvider provider) : ICommand {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  public string Name => "create";
  public string Description => "Creates a new gang";
  public string[] Usage => ["[name]"];
  private const int CREATION_COST = 500;

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;

    if (info.ArgCount < 2) return CommandResult.PRINT_USAGE;

    var name = string.Join(' ', info.Args.Skip(1));

    if (await gangs.GetGang(executor.Steam) != null) {
      info.ReplySync(locale.Get(MSG.ALREADY_IN_GANG));
      return CommandResult.ERROR;
    }

    if ((await gangs.GetGangs()).Any(g => g.Name == name)) {
      info.ReplySync(locale.Get(MSG.COMMAND_GANG_CREATE_ALREADY_EXISTS, name));
      return CommandResult.ERROR;
    }

    if (await eco.TryPurchase(executor, CREATION_COST, item: "Create") < 0)
      return CommandResult.SUCCESS;

    var newGang = await gangs.CreateGang(name, executor.Steam);
    if (newGang == null) {
      info.ReplySync(locale.Get(MSG.GENERIC_ERROR_INFO,
        "Failed to create a gang"));
      return CommandResult.ERROR;
    }

    info.ReplySync(locale.Get(MSG.COMMAND_GANG_CREATE_SUCCESS, name,
      newGang.GangId));
    return CommandResult.SUCCESS;
  }
}