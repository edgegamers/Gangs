using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Perks;
using GangsAPI.Services;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;
using IMenu = GangsAPI.Services.Menu.IMenu;

namespace Stats.Perk.Display;

public class DisplayPerkMenu(IServiceProvider provider, DisplayData data)
  : IMenu {
  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

  private readonly IEcoManager economy =
    provider.GetRequiredService<IEcoManager>();

  public async Task Open(PlayerWrapper player) {
    player.PrintToChat(
      $" {ChatColors.DarkBlue}Gang Perk: {ChatColors.Blue}Display");
    player.PrintToChat(
      $" {ChatColors.LightBlue}Display your gang's name either in chat or the scoreboard");

    var displayPerk    = provider.GetService<IDisplayPerk>();
    var displaySetting = provider.GetService<IDisplaySetting>();

    if (data.ChatBought && displaySetting != null && displayPerk != null) {
      var enabled = await displaySetting.IsChatEnabled(player.Steam);
      var word    = enabled ? "Enabled" : "Disabled";
      player.PrintToChat($"1. {ChatColors.Green}Chat ({word})");
    } else if (displayPerk != null) {
      var canAfford = await economy.CanAfford(player, displayPerk.ChatCost);
      var color     = canAfford ? ChatColors.Green : ChatColors.Red;
      player.PrintToChat($"1. {color}Chat ({displayPerk.ChatCost})");
    }

    if (data.ScoreboardBought && displaySetting != null) {
      var enabled = await displaySetting.IsScoreboardEnabled(player.Steam);
      var word    = enabled ? "Enabled" : "Disabled";
      player.PrintToChat($"2. {ChatColors.Green} Scoreboard ({word})");
    } else if (displayPerk != null) {
      var canAfford =
        await economy.CanAfford(player, displayPerk.ScoreboardCost);
      var color = canAfford ? ChatColors.Green : ChatColors.Red;
      player.PrintToChat(
        $"2. {color}Scoreboard ({displayPerk.ScoreboardCost})");
    }
  }

  public Task Close(PlayerWrapper player) { return Task.CompletedTask; }

  public Task AcceptInput(PlayerWrapper player, int input) {
    if (input > 2) return Task.CompletedTask;
    commands.ProcessCommand(player, CommandCallingContext.Chat, "css_gang",
      "display", $"{input - 1}");
    return Task.CompletedTask;
  }
}