﻿using GangsAPI.Data.Stat;

namespace Stats;

public class DescriptionStat : BaseStat<string?> {
  public override string StatId => "gang_native_description";
  public override string Name => "Description";
  public override string Description => "The description of the gang.";

  public override IStat<string?> Clone() {
    return new DescriptionStat { Value = Value };
  }
}