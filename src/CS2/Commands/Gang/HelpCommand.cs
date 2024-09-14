using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

namespace Commands.Gang;

public class HelpCommand(IServiceProvider provider,
  IDictionary<string, ICommand> sub) : ICommand {
  public string Name => "help";
  public string Description => "Displays help for gangs";

  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    HashSet<string> sent = new();

    foreach (var (name, cmd) in sub) {
      if (!sent.Add(cmd.Name)) continue;
      if (cmd.Description != null)
        info.ReplySync(locale.Get(MSG.COMMAND_USAGE,
          $"{cmd.Name} - {cmd.Description}"));
      else
        info.ReplySync(locale.Get(MSG.COMMAND_USAGE, $"{cmd.Name}"));

      foreach (var use in cmd.Usage)
        info.ReplySync($" - {ChatColors.Grey}{cmd.Name} {use}");
    }

    return Task.FromResult(CommandResult.SUCCESS);
  }
}