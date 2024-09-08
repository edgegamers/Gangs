using GangsAPI.Services.Commands;

namespace GangsTest.API.Services.Commands.Command;

public class FieldTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public void Fields_Name(ICommand cmd) {
    Assert.NotEmpty(cmd.Name);
    // Assert.False(cmd.Usage.StartsWith(cmd.Name),
    //   "Command usage should not start with the command name");
    Assert.DoesNotContain(cmd.Usage, usage => usage.StartsWith(cmd.Name));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public void Fields_Aliases(ICommand cmd) {
    Assert.Contains(cmd.Name, cmd.Aliases);
  }
}