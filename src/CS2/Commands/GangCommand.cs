using Commands.Gang;
using Commands.Menus;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.Localization;
using static GangsAPI.MSG;

namespace Commands;

public class GangCommand(IGangManager gangMgr, IPlayerManager playerMgr,
  IMenuManager menuMgr, IStringLocalizer testLocale) : ICommand {
  private readonly Dictionary<string, ICommand> sub = new() {
    // ["delete"] = new DeleteGangCommand(),
    // ["invite"] = new InviteGangCommand(),
    // ["kick"] = new KickGangCommand(),
    // ["leave"] = new LeaveGangCommand(),ggG
    // ["list"] = new ListGangCommand(),
    // ["promote"] = new PromoteGangCommand(),
    // ["demote"] = new DemoteGangCommand(),
    // ["info"] = new InfoGangCommand()
    ["create"] = new CreateCommand(gangMgr, testLocale),
    ["help"]   = new HelpCommand()
  };

  private IStringLocalizer locale = testLocale;

  public void Start(BasePlugin? plugin, bool hotReload) {
    if (plugin != null) locale = plugin?.Localizer ?? locale;
  }

  public string Name => "css_gang";
  public string Description => "Master command for gangs";
  public string[] Aliases => ["css_gang", "css_gangs"];

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (info.ArgCount == 0)
      throw new InvalidOperationException(
        "Attempted to execute GangCommand with no arguments");
    if (!Aliases.Contains(info[0]))
      throw new InvalidOperationException(
        $"Attempted to execute GangCommand with invalid name: {info[0]}");

    if (info.ArgCount == 1) {
      if (executor == null) return CommandResult.PLAYER_ONLY;
      var gang = await gangMgr.GetGang(executor.Steam);

      if (gang == null) {
        info.ReplySync(locale[COMMAND_GANG_NOTINGANG.Key()]);
        return CommandResult.SUCCESS;
      }

      // Open gang menu
      // var menu = await new GangMenu(gang, playerMgr).Load();
      // await Server.NextFrameAsync(() => menu.Open(executor.Player));
      await menuMgr.OpenMenu(executor, new GangMenu(gang, playerMgr));

      return CommandResult.SUCCESS;
    }

    if (!sub.TryGetValue(info[1], out var command))
      return CommandResult.UNKNOWN_COMMAND;

    var newInfo = new CommandInfoWrapper(info, 1);
    return await command.Execute(executor, newInfo);
  }
}