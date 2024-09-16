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
  private readonly IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  public string Name => "help";
  public string Description => "Displays help for gangs";

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    HashSet<string> sent = [];

    foreach (var (name, cmd) in sub) {
      if (!sent.Add(cmd.Name)) continue;
      var uses = cmd.Usage.Where(use => !string.IsNullOrEmpty(use)).ToList();
      var useString = uses.Count > 0 ? "(" + string.Join(", ", uses) + ")" : "";
      if (cmd.Description != null)
        info.ReplySync(locale.Get(MSG.PREFIX) + ChatColors.Grey
          + $"{cmd.Name} - {cmd.Description}");
      else
        info.ReplySync(locale.Get(MSG.PREFIX) + $"{cmd.Name} {useString}");
    }

    return Task.FromResult(CommandResult.SUCCESS);
  }
}