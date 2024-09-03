using GangsAPI.Services.Commands;

namespace GangsTest.Commands;

public class FieldTests {
  [Theory]
  [ClassData(typeof(CommandTestData))]
  public void Command_Fields(ICommand cmd) {
    Assert.NotEmpty(cmd.Name);
    Assert.False(cmd.Usage.StartsWith(cmd.Name),
      "Command usage should not start with the command name");
  }
}