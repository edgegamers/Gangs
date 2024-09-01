using GangsAPI.Data.Stat;

namespace Mock;

public class MockBankStat : IGangStat<int> {
  public string StatId => "gang_bank";
  public string Name => "Mock Bank";
  public string? Description => "The balance of the gang's bank.";
  public int Value { get; set; } = 0;
}