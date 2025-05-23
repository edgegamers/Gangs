﻿using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using GangsAPI.Data;
using GangsAPI.Extensions;
using GangsAPI.Menu;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace Stats.Perk.Smoke;

public class SmokeColorMenu(IServiceProvider provider, SmokePerkData data)
  : AbstractPagedMenu<SmokeColor>(provider, NativeSenders.Chat) {
  private readonly ICommandManager commands =
    provider.GetRequiredService<ICommandManager>();

  // Method to sort smoke colors
  private int CompareSmokeColors(SmokeColor a, SmokeColor b) {
    // If the icon is equipped, it should be first
    if (a == data.Equipped) return -1;
    if (b == data.Equipped) return 1;

    // If icon is unlocked, it should be next
    // If both are unlocked, sort by cost (highest first)
    if (data.Unlocked.HasFlag(a)) {
      if (data.Unlocked.HasFlag(b)) return a.GetCost().CompareTo(b.GetCost());
      return -1;
    }

    // If both are locked, sort by cost (lowest first)
    if (data.Unlocked.HasFlag(b)) return 1;
    return a.GetCost().CompareTo(b.GetCost());
  }


  override protected Task<List<SmokeColor>> GetItems(PlayerWrapper player) {
    var list = Enum.GetValues<SmokeColor>().ToList();
    list.Sort(CompareSmokeColors);
    list.Insert(0, 0);
    return Task.FromResult(list);
  }

  override protected Task HandleItemSelection(PlayerWrapper player,
    List<SmokeColor> items, int selectedIndex) {
    if (selectedIndex > 0)
      commands.ProcessCommand(player, CommandCallingContext.Chat, "css_gang",
        "smokecolor", items[selectedIndex].ToString());
    Close(player);
    return Task.CompletedTask;
  }

  override protected Task<string> FormatItem(PlayerWrapper player, int index,
    SmokeColor item) {
    var name = item.ToString().ToTitleCase();
    if (item == 0)
      return Task.FromResult(
        $" {ChatColors.DarkBlue}Gang Perks: {ChatColors.LightBlue}Smoke Colors");
    if (item == data.Equipped)
      return Task.FromResult(
        $"{index}. {item.GetChatColor()}{name} {ChatColors.Green}({ChatColors.Lime}Equipped{ChatColors.Green})");
    if (data.Unlocked.HasFlag(item))
      return Task.FromResult(
        $"{index}. {item.GetChatColor()}{name} {ChatColors.Green}({ChatColors.Grey}Unlocked{ChatColors.Green})");
    return Task.FromResult(
      $"{index}. {item.GetChatColor()}{name} {ChatColors.DarkRed}({ChatColors.LightRed}{item.GetCost()}{ChatColors.DarkRed})");
  }
}