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
    var primitive =
      stat.ValueType.IsPrimitive || stat.ValueType == typeof(string);
    var constructor = stat.ValueType.GetConstructor([]);
    Assert.True(primitive || constructor is not null);
  }
}