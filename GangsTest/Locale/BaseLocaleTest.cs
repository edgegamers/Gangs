using System.Diagnostics;
using System.Text.RegularExpressions;
using GangsAPI;
using Microsoft.Extensions.Localization;
using Xunit.Abstractions;

namespace GangsTest.Locale;

public partial class BaseLocaleTest(IStringLocalizer localizer,
  ITestOutputHelper testOutputHelper) {
  [Fact]
  public void Prefix_Exists() {
    var prefix = localizer["prefix"];
    Assert.False(string.IsNullOrEmpty(prefix.Value));
    testOutputHelper.WriteLine(prefix.Value);
  }

  [Fact]
  public void Prefix_Replacement() {
    Assert.StartsWith(localizer["prefix"].Value.TrimStart(),
      localizer["command.gang.not_in_gang"].ToString().TrimStart());
  }

  [Theory]
  [ClassData(typeof(LocaleKeyValueData))]
  public void Brackets_Contain_DigitsOnly(string _, string val) {
    // For all the bracket matches, make sure the internal string is only digits
    foreach (Match match in ColorBrackets().Matches(val))
      Assert.Matches(@"^\d+$", match.Groups[1].Value);
  }

  [Theory]
  [ClassData(typeof(LocaleKeyValueData))]
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
  [ClassData(typeof(LocaleKeyValueData))]
  public void No_Empty_Values(string key, string val) {
    Assert.False(string.IsNullOrEmpty(val));
  }

  [Theory]
  [ClassData(typeof(LocaleKeyValueData))]
  public void Ends_In_Punctuation(string key, string val) {
    if (key == "prefix")
      return; // TODO: Once xUnit 3.0 is released, use Assert.Skip
    Assert.Matches(@"[.,!?]$", val);
  }

  [Theory]
  [ClassData(typeof(LocaleImplData))]
  public void Localize_Exists_FromImpl(string key) {
    Assert.False(string.IsNullOrEmpty(localizer[key]));
  }

  [Theory]
  [ClassData(typeof(LocaleKeyValueData))]
  public void Localize_Exists_FromFile(string key, string _) {
    // Make sure we have an enum whose key is the same as the key in the file
    Assert.Contains(key, localeKeys);
  }

  private readonly IEnumerable<string> localeKeys =
    Enum.GetValues<GangsAPI.Locale>().Select(s => s.Key());

  [GeneratedRegex("{(.*?)}")]
  private static partial Regex ColorBrackets();
}