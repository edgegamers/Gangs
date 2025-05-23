﻿using System.Drawing;
using CounterStrikeSharp.API.Modules.Utils;

namespace Stats.Perk.Smoke;

[Flags]
public enum SmokeColor {
  RED = 1 << 0,
  ORANGE = 1 << 1,
  YELLOW = 1 << 2,
  GREEN = 1 << 3,
  CYAN = 1 << 4,
  BLUE = 1 << 5,
  PURPLE = 1 << 6,
  DEFAULT = 1 << 7,
  RANDOM = 1 << 8
}

public static class SmokeColorExtensions {
  public static int GetCost(this SmokeColor color) {
    return color switch {
      SmokeColor.RED     => 7500,
      SmokeColor.ORANGE  => 3750,
      SmokeColor.YELLOW  => 3125,
      SmokeColor.GREEN   => 2500,
      SmokeColor.CYAN    => 6250,
      SmokeColor.BLUE    => 5000,
      SmokeColor.PURPLE  => 5625,
      SmokeColor.DEFAULT => 0,
      SmokeColor.RANDOM  => 12500,
      _                  => 0
    };
  }

  public static Color? GetColor(this SmokeColor color) {
    return color switch {
      SmokeColor.RED     => Color.Red,
      SmokeColor.ORANGE  => Color.Orange,
      SmokeColor.YELLOW  => Color.Yellow,
      SmokeColor.GREEN   => Color.Green,
      SmokeColor.CYAN    => Color.LightBlue,
      SmokeColor.BLUE    => Color.Blue,
      SmokeColor.PURPLE  => Color.Purple,
      SmokeColor.DEFAULT => null,
      SmokeColor.RANDOM  => null,
      _                  => Color.White
    };
  }

  public static char GetChatColor(this SmokeColor color) {
    return color switch {
      SmokeColor.RED     => ChatColors.Red,
      SmokeColor.ORANGE  => ChatColors.Orange,
      SmokeColor.YELLOW  => ChatColors.Yellow,
      SmokeColor.GREEN   => ChatColors.Green,
      SmokeColor.CYAN    => ChatColors.LightBlue,
      SmokeColor.BLUE    => ChatColors.Blue,
      SmokeColor.PURPLE  => ChatColors.Purple,
      SmokeColor.DEFAULT => ChatColors.White,
      SmokeColor.RANDOM  => ChatColors.White,
      _                  => ChatColors.White
    };
  }

  public static Color? PickRandom(this SmokeColor color) {
    var n = new Random().Next(Enum.GetValues<SmokeColor>().Length);
    var available = Enum.GetValues<SmokeColor>()
     .Where(c => color.HasFlag(c) && c.GetColor() != null)
     .ToList();

    // Gang bought the random perk, but no colors, sillies!
    if (available.Count == 0) return null;

    return available[n % available.Count].GetColor();
  }
}