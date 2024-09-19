using Commands.Gang;
using Commands.Menus;
using CounterStrikeSharp.API.Core;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Menu;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using static GangsAPI.MSG;

namespace Commands;

public class GangCommand(IServiceProvider provider) : ICommand {
  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IMenuManager menus =
    provider.GetRequiredService<IMenuManager>();

  private readonly Dictionary<string, ICommand> sub = new() {
    ["members"]     = new MembersCommand(provider),
    ["perks"]       = new PerksCommand(provider),
    ["invites"]     = new InvitesCommand(provider),
    ["invite"]      = new InviteCommand(provider),
    ["create"]      = new CreateCommand(provider),
    ["deposit"]     = new DepositCommand(provider),
    ["purchase"]    = new PurchaseCommand(provider),
    ["balance"]     = new Gang.BalanceCommand(provider),
    ["credits"]     = new Gang.BalanceCommand(provider),
    ["disband"]     = new DisbandCommand(provider),
    ["motd"]        = new MotdCommand(provider),
    ["description"] = new MotdCommand(provider),
    ["promote"]     = new PromoteCommand(provider),
    ["demote"]      = new DemoteCommand(provider),
    ["doorpolicy"]  = new DoorPolicyCommand(provider),
    ["stats"]       = new Gang.StatsCommand(),
    ["perms"]       = new PermissionCommand(provider),
    ["permissions"] = new PermissionCommand(provider),
    ["permission"]  = new PermissionCommand(provider),
    ["rank"]        = new RankCommand(provider),
    ["ranks"]       = new RankCommand(provider),
    ["kick"]        = new KickCommand(provider),
    ["display"]     = new DisplayCommand(provider),
    ["transfer"]    = new TransferCommand(provider),
    ["leave"]       = new LeaveCommand(provider),
    ["smokecolor"]  = new SmokeColorCommand(provider),
    ["join"]        = new JoinCommand(provider)
  };

  private IStringLocalizer locale =
    provider.GetRequiredService<IStringLocalizer>();

  public void Start(BasePlugin? plugin, bool hotReload) {
    if (plugin != null) locale = plugin.Localizer;
    sub["help"] = new HelpCommand(provider, sub);

    foreach (var cmd in sub.Values) cmd.Start(plugin, hotReload);
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
      var gang = await gangs.GetGang(executor.Steam);

      if (gang == null) {
        info.ReplySync(locale.Get(COMMAND_GANG_NOTINGANG));
        return CommandResult.SUCCESS;
      }

      // Open gang menu
      await menus.OpenMenu(executor, new GangMenu(provider, gang));
      return CommandResult.SUCCESS;
    }

    if (int.TryParse(info[1], out var index)) {
      if (executor == null) return CommandResult.PLAYER_ONLY;
      var gang = await gangs.GetGang(executor.Steam);

      if (gang == null) {
        info.ReplySync(locale.Get(NOT_IN_GANG));
        return CommandResult.SUCCESS;
      }

      var menu = new GangMenu(provider, gang);
      await menu.AcceptInput(executor, index);
    }

    if (!sub.TryGetValue(info[1], out var command))
      return CommandResult.UNKNOWN_COMMAND;

    var newInfo =
      new CommandInfoWrapper(executor, 1, info.Args) {
        CallingContext = info.CallingContext
      };

    var result = await command.Execute(executor, newInfo);
    if (result == CommandResult.PRINT_USAGE)
      foreach (var use in command.Usage)
        info.ReplySync(
          locale.Get(COMMAND_USAGE, $"{Name} {command.Name} {use}"));

    return result;
  }
}