using System.Collections;
using GangsAPI;
using Mock;
using SQLImpl;

namespace GangsTest.StatTests.InstanceManageTests;

public class InstanceManageData : IEnumerable<object[]> {
  private readonly IBehavior[] behaviors = [
    new MockInstanceStatManager(),
    new SQLInstanceManager(
      Environment.GetEnvironmentVariable("DB_GANGS_CONNECTION")
      ?? "Host=localhost;User=root;Database=gangs", "gang_inst_stats", "", true)
  ];

  public InstanceManageData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => new[] { behavior }).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}