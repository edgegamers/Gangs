using GangsAPI.Data.Stat;

namespace GangsTest.StatTests.InstanceTests;

public class InstanceFieldTests {
  [Theory]
  [ClassData(typeof(StatInstanceData))]
  public void Instance_Id(IStat stat) {
    Assert.Matches("^[a-z0-9_]+$", stat.StatId);
  }
}