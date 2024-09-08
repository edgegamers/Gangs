using System.Collections;
using GangsAPI.Services.Player;
using Mock;
using SQLImpl;

namespace GangsTest.API.Services.Stat.Instance.Player;

public class TestData : IEnumerable<object[]> {
  private readonly IPlayerStatManager[] behaviors = [
    new MockInstanceStatManager(),
    new SQLPlayerInstanceManager(
      Environment.GetEnvironmentVariable("DB_GANGS_CONNECTION")
      ?? "Host=localhost;User=root;Database=gangs", "gang_inst_stats", true),
    new SQLPlayerInstanceManager(
      Environment.GetEnvironmentVariable("DB_GANGS_CONNECTION")
      ?? "Host=localhost;User=root;Database=gangs", "gang_inst_stats", true)
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => new[] { behavior }).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}