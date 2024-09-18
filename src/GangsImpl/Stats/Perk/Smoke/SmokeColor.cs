using System.Drawing;
using CounterStrikeSharp.API.Modules.Utils;

namespace Stats.Perk.Smoke;

[Flags]
public enum SmokeColor {
  DARK_RED = 1 << 0,
  RED = 1 << 1,
  ORANGE = 1 << 2,
  YELLOW = 1 << 3,
  LIGHT_GREEN = 1 << 4,
  GREEN = 1 << 5,
  LIGHT_BLUE = 1 << 6,
  BLUE = 1 << 7,
  MAGENTA = 1 << 8,
  PURPLE = 1 << 9,
  BLACK = 1 << 10,
  WHITE = 1 << 11,
  DEFAULT = 1 << 12,
  RANDOM = 1 << 13
}

public static class SmokeColorExtensions {
  public static int GetCost(this SmokeColor color) {
    return color switch {
      SmokeColor.DARK_RED    => 5000,
      SmokeColor.RED         => 2000,
      SmokeColor.ORANGE      => 1000,
      SmokeColor.YELLOW      => 1000,
      SmokeColor.LIGHT_GREEN => 1500,
      SmokeColor.GREEN       => 2000,
      SmokeColor.LIGHT_BLUE  => 3000,
      SmokeColor.BLUE        => 4000,
      SmokeColor.MAGENTA     => 5000,
      SmokeColor.PURPLE      => 1000,
      SmokeColor.WHITE       => 1000,
      SmokeColor.DEFAULT     => 1000,
      SmokeColor.RANDOM      => 10000,
      _                      => 0
    };
  }

  public static Color? GetColor(this SmokeColor color) {
    return color switch {
      SmokeColor.DARK_RED    => Color.DarkRed,
      SmokeColor.RED         => Color.Red,
      SmokeColor.ORANGE      => Color.Orange,
      SmokeColor.YELLOW      => Color.Yellow,
      SmokeColor.LIGHT_GREEN => Color.LightGreen,
      SmokeColor.GREEN       => Color.Green,
      SmokeColor.LIGHT_BLUE  => Color.LightBlue,
      SmokeColor.BLUE        => Color.Blue,
      SmokeColor.MAGENTA     => Color.Magenta,
      SmokeColor.PURPLE      => Color.Purple,
      SmokeColor.BLACK       => Color.Black,
      SmokeColor.WHITE       => Color.White,
      SmokeColor.DEFAULT     => null,
      SmokeColor.RANDOM      => null,
      _                      => Color.White
    };
  }

  public static char GetChatColor(this SmokeColor color) {
    return color switch {
      SmokeColor.DARK_RED    => ChatColors.DarkRed,
      SmokeColor.RED         => ChatColors.Red,
      SmokeColor.ORANGE      => ChatColors.Orange,
      SmokeColor.YELLOW      => ChatColors.Yellow,
      SmokeColor.LIGHT_GREEN => ChatColors.Lime,
      SmokeColor.GREEN       => ChatColors.Green,
      SmokeColor.LIGHT_BLUE  => ChatColors.LightBlue,
      SmokeColor.BLUE        => ChatColors.Blue,
      SmokeColor.MAGENTA     => ChatColors.Magenta,
      SmokeColor.PURPLE      => ChatColors.Purple,
      SmokeColor.BLACK       => ChatColors.Grey,
      SmokeColor.WHITE       => ChatColors.White,
      SmokeColor.DEFAULT     => ChatColors.White,
      SmokeColor.RANDOM      => ChatColors.White,
      _                      => ChatColors.White
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