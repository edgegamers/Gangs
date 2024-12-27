using System.Diagnostics;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI;
using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Perks;
using GangsAPI.Permissions;
using GangsAPI.Services;
using GangsAPI.Services.Gang;
using GangsAPI.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Commands.Gang;

public class DisplayCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  private readonly IEcoManager eco = provider.GetRequiredService<IEcoManager>();

  private readonly IGangManager gangs =
    provider.GetRequiredService<IGangManager>();

  private readonly IRankManager ranks =
    provider.GetRequiredService<IRankManager>();

  public override string Name => "display";
  public override string[] Usage => ["<0/1>"];

  public override string Description
    => "Show your gang in chat or the scoreboard";

  override protected async Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    if (info.ArgCount != 2) return CommandResult.PRINT_USAGE;

    var display = info.Args[1].ToLower() switch {
      "0" or "chat" or "c"       => 0,
      "1" or "scoreboard" or "s" => 1,
      _                          => -1
    };

    if (display == -1) return CommandResult.PRINT_USAGE;

    var perk = Provider.GetService<IDisplayPerk>()
      ?? throw new GangException("Display perk not found");

    Debug.Assert(player.GangId != null, "player.GangId != null");
    var gang = await gangs.GetGang(player.GangId.Value)
      ?? throw new GangNotFoundException(player);

    var (canBuy, required) = await ranks.CheckRank(player, Perm.PURCHASE_PERKS);

    if (display == 0) {
      if (!await perk.HasChatDisplay(gang)) {
        if (!canBuy) {
          info.ReplySync(Locale.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
          return CommandResult.NO_PERMISSION;
        }

        if (await eco.TryPurchase(executor, perk.ChatCost, item: "Chat Display")
          < 0)
          return CommandResult.SUCCESS;

        await perk.SetChatDisplay(gang, true);

        var gangChat = Provider.GetService<IGangChatPerk>();
        if (gangChat == null) return CommandResult.SUCCESS;

        await gangChat.SendGangChat(player, gang,
          Locale.Get(MSG.PERK_PURCHASED, player.Name ?? player.Steam.ToString(),
            "Display: Chat"));
        return CommandResult.SUCCESS;
      }

      // Toggle
      var displaySetting = Provider.GetService<IDisplaySetting>()
        ?? throw new GangException("Display setting not found");
      var enabled = await displaySetting.IsChatEnabled(player.Steam);
      await displaySetting.SetChatEnabled(player.Steam, !enabled);
      info.ReplySync(Locale.Get(MSG.PERK_DISPLAY_CHAT,
        enabled ? ChatColors.Red + "disabled" : ChatColors.Green + "enabled"));
      await updateDisplay(executor, gang.Name, !enabled ? 1 : 0);
      return CommandResult.SUCCESS;
    }

    if (!await perk.HasScoreboardDisplay(gang)) {
      if (!canBuy) {
        info.ReplySync(Locale.Get(MSG.GENERIC_NOPERM_RANK, required.Name));
        return CommandResult.NO_PERMISSION;
      }

      if (await eco.TryPurchase(executor, perk.ScoreboardCost,
        item: "Scoreboard Display") < 0)
        return CommandResult.SUCCESS;

      await perk.SetScoreboardDisplay(gang, true);

      var gangChat = Provider.GetService<IGangChatPerk>();
      if (gangChat == null) return CommandResult.SUCCESS;

      await gangChat.SendGangChat(player, gang,
        Locale.Get(MSG.PERK_PURCHASED, player.Name ?? player.Steam.ToString(),
          "Display: Scoreboard"));
      return CommandResult.SUCCESS;
    }

    // Toggle
    var scoreboardSetting = Provider.GetService<IDisplaySetting>()
      ?? throw new GangException("Display setting not found");
    var scoreboardEnabled =
      await scoreboardSetting.IsScoreboardEnabled(player.Steam);
    await scoreboardSetting.SetScoreboardEnabled(player.Steam,
      !scoreboardEnabled);
    info.ReplySync(Locale.Get(MSG.PERK_DISPLAY_SCOREBOARD,
      scoreboardEnabled ?
        ChatColors.Red + "disabled" :
        ChatColors.Green + "enabled"));

    await updateDisplay(executor, gang.Name, -1, !scoreboardEnabled ? 1 : 0);
    return CommandResult.SUCCESS;
  }

  private Task updateDisplay(PlayerWrapper player, string name, int chat = -1,
    int scoreboard = -1) {
    if (player.Player == null) return Task.CompletedTask;
    return Server.NextFrameAsync(() => {
      if (!player.Player.IsValid) return;
      if (chat != -1) {
        var tags = ThirdPartyAPI.Actain?.getTagService();
        tags?.SetTag(player.Player, chat == 0 ? "" : name, false);
      }

      if (scoreboard == -1) return;
      Debug.Assert(player.Player != null, "player.Player != null");
      player.Player.Clan = scoreboard == 0 ? "" : name;
      Utilities.SetStateChanged(player.Player, "CCSPlayerController",
        "m_szClan");
      var ev = new EventNextlevelChanged(true);
      ev.FireEvent(false);
    });
  }
}