using GangsAPI.Data.Stat;

namespace GangsTest.API.Services.Stat.Concrete;

public class ConcreteTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public void Instance_Id(IStat stat) {
    Assert.Matches("^[a-z0-9_]+$", stat.StatId);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public void Empty_Constructor(IStat stat) {
    Assert.NotNull(stat.GetType().GetConstructor([]));
  }
}