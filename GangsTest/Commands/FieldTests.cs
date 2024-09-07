using GangsAPI.Services.Commands;

namespace GangsTest.Commands;

public class FieldTests {
  [Theory]
  [ClassData(typeof(CommandsData))]
  public void Fields_Name(ICommand cmd) {
    Assert.NotEmpty(cmd.Name);
    Assert.False(cmd.Usage.StartsWith(cmd.Name),
      "Command usage should not start with the command name");
  }

  [Theory]
  [ClassData(typeof(CommandsData))]
  public void Fields_Aliases(ICommand cmd) {
    Assert.Contains(cmd.Name, cmd.Aliases);
  }
}