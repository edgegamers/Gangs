using System.Drawing;
using CounterStrikeSharp.API.Modules.Utils;

namespace Stats.Perk.Smoke;

[Flags]
public enum SmokeColor {
  RED = 1 << 0,
  ORANGE = 1 << 1,
  YELLOW = 1 << 2,
  GREEN = 1 << 3,
  LIGHT_BLUE = 1 << 4,
  BLUE = 1 << 5,
  PURPLE = 1 << 6,
  DEFAULT = 1 << 7,
  RANDOM = 1 << 8,
  RAINBOW = 1 << 9
}

public static class SmokeColorExtensions {
  public static int GetCost(this SmokeColor color) {
    return color switch {
      SmokeColor.RED     => 2000,
      SmokeColor.ORANGE  => 1000,
      SmokeColor.YELLOW  => 1000,
      SmokeColor.GREEN   => 2000,
      SmokeColor.BLUE    => 4000,
      SmokeColor.PURPLE  => 2000,
      SmokeColor.DEFAULT => 1000,
      SmokeColor.RANDOM  => 10000,
      SmokeColor.RAINBOW => 25000,
      _                  => 0
    };
  }

  public static Color? GetColor(this SmokeColor color) {
    return color switch {
      SmokeColor.RED     => Color.Red,
      SmokeColor.ORANGE  => Color.Orange,
      SmokeColor.YELLOW  => Color.Yellow,
      SmokeColor.GREEN   => Color.Green,
      SmokeColor.BLUE    => Color.Blue,
      SmokeColor.PURPLE  => Color.Purple,
      SmokeColor.DEFAULT => null,
      SmokeColor.RANDOM  => null,
      SmokeColor.RAINBOW => Color.White,
      _                  => Color.White
    };
  }

  public static char GetChatColor(this SmokeColor color) {
    return color switch {
      SmokeColor.RED        => ChatColors.Red,
      SmokeColor.ORANGE     => ChatColors.Orange,
      SmokeColor.YELLOW     => ChatColors.Yellow,
      SmokeColor.GREEN      => ChatColors.Green,
      SmokeColor.LIGHT_BLUE => ChatColors.LightBlue,
      SmokeColor.BLUE       => ChatColors.Blue,
      SmokeColor.PURPLE     => ChatColors.Purple,
      SmokeColor.DEFAULT    => ChatColors.White,
      SmokeColor.RANDOM     => ChatColors.White,
      SmokeColor.RAINBOW    => ChatColors.Orange,
      _                     => ChatColors.White
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