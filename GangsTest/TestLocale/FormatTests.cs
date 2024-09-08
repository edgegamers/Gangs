using System.Text.RegularExpressions;
using GangsAPI;
using Microsoft.Extensions.Localization;
using Xunit.Abstractions;

namespace GangsTest.TestLocale;

public partial class FormatTests(IStringLocalizer localizer,
  ITestOutputHelper output) {
  [Fact]
  public void Handles_Formatting() {
    var perm   = "@test/permission";
    var result = localizer[MSG.GENERIC_NOPERM_NODE.Key(), perm].Value;
    Assert.Contains(perm, result);
  }

  [Theory]
  [ClassData(typeof(LocaleFileKVData))]
  public void Brackets_Contain_DigitsOnly(string _, string val) {
    // For all the bracket matches, make sure the internal string is only digits
    foreach (Match match in ColorBrackets().Matches(val))
      Assert.Matches(@"^\d+$", match.Groups[1].Value);
  }

  [Theory]
  [ClassData(typeof(LocaleFileKVData))]
  public void Brackets_Open_And_Close(string _, string val) {
    // For each opening bracket, make sure there is a closing bracket
    var brackets = 0;
    foreach (var c in val) {
      if (c == '{') brackets++;
      if (c == '}') brackets--;
    }

    Assert.Equal(0, brackets);
  }

  [Theory]
  [ClassData(typeof(LocaleFileKVData))]
  public void No_Empty_Values(string _, string val) {
    Assert.False(string.IsNullOrEmpty(val));
  }

  [Theory]
  [ClassData(typeof(LocaleFileKVData))]
  public void Keys_Use_Subchars(string key, string _) {
    Assert.Matches(@"^[a-z_\.]+$", key);
  }

  [Theory]
  [ClassData(typeof(LocaleFileKVData))]
  public void Ends_In_Punctuation(string key, string val) {
    if (key == "prefix")
      return; // TODO: Once xUnit 3.0 is released, use Assert.Skip
    Assert.Matches(@"[.,!?]$", val);
  }

  [GeneratedRegex("{(.*?)}")]
  private static partial Regex ColorBrackets();
}