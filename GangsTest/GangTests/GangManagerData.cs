using System.Collections;
using GangsAPI;
using Mock;
using SQLImpl;

namespace GangsTest.GangTests;

public class GangManagerData : IEnumerable<object[]> {
  private readonly IBehavior[] behaviors = [
    new MockGangManager(),
    new SQLGangManager(
      Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
      ?? "Host=localhost;User=root;Database=gangs", "gang_unit_test", true)
  ];

  public GangManagerData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}