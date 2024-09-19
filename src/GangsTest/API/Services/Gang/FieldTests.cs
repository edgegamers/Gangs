using GangsAPI.Services.Gang;

namespace GangsTest.API.Services.Gang;

public class FieldTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Gang_Fields_Name(IGangManager mgr) {
    var dummy = await mgr.CreateGang("name", 0);
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
  }
}