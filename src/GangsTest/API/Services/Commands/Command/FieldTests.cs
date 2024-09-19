using GangsAPI.Services.Commands;

namespace GangsTest.API.Services.Commands.Command;

public class FieldTests {
  [Theory]
  [ClassData(typeof(TestData))]
  public void Usages_AllUsages_DoNotStartWithName(ICommand cmd) {
    Assert.NotEmpty(cmd.Name);
    Assert.DoesNotContain(cmd.Usage, usage => usage.StartsWith(cmd.Name));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public void Aliases_AllAliases_ContainCommandName(ICommand cmd) {
    Assert.Contains(cmd.Name, cmd.Aliases);
  }
}