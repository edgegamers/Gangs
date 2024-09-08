using Microsoft.Extensions.Localization;
using Xunit.Abstractions;

namespace GangsTest.TestLocale;

public class PrefixTests(IStringLocalizer localizer, ITestOutputHelper output) {
  [Fact]
  public void Exists() {
    var prefix = localizer["prefix"];
    Assert.False(string.IsNullOrEmpty(prefix.Value));
    output.WriteLine(prefix.Value);
  }

  [Fact]
  public void Replaces() {
    Assert.StartsWith(localizer["prefix"].Value.TrimStart(),
      localizer["command.gang.not_in_gang"].ToString().TrimStart());
  }
}