using GangsAPI.Services.Commands;

namespace GangsTest.API.Services.Commands.Command;

public class FieldTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public void Fields_Aliases(ICommand cmd) {
    Assert.Contains(cmd.Name, cmd.Aliases);
  }
}