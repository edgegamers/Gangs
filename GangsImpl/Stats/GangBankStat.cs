using GangsAPI.Data.Stat;

namespace Stats;

public class GangBankStat : IStat<int> {
  public string StatId => "gang_native_bank";
  public string Name => "Gang Bank";
  public string? Description => "The amount of money in the gang's bank.";
  public int Value { get; set; }

  public IStat<int> Clone() { return new GangBankStat { Value = Value }; }

  public bool Equals(IStat? other) {
    return other is not null && StatId == other.StatId;
  }

  public bool Equals(IStat<int>? other) {
    return other is not null && StatId == other.StatId;
  }
}