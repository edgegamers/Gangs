using System.Text.RegularExpressions;
using GangsAPI;
using Microsoft.Extensions.Localization;

namespace GangsTest.TestLocale;

public partial class FormatTests(IStringLocalizer localizer) {
  [Fact]
  public void Handles_Formatting() {
    var perm   = "@test/permission";
    var result = localizer[MSG.GENERIC_NOPERM_NODE.Key(), perm].Value;
    Assert.Contains(perm, result);
  }

  [Fact]
  public void FileLocalizer_WithPercentPlurals_HandlesSimpleWord() {
    Assert.EndsWith("s.", localizer.Get(MSG.COMMAND_BALANCE_NONE));
  }

  [Fact]
  public void FileLocalizer_WithPercentPlurals_HandlesOne() {
    var currencyName = localizer.Get(MSG.CURRENCY);
    Assert.EndsWith(currencyName + ".", localizer.Get(MSG.COMMAND_BALANCE, 1));
  }

  [Theory]
  [InlineData("You don't have any credits", "You don't have any credit%s%")]
  [InlineData("Peanuts", "Peanut%s%")]
  public void
    StaticLocalizer_WithPercentPlurals_HandlesPlurals(string translated,
      string source) {
    Assert.Equal(translated, StringLocalizer.HandlePluralization(source));
  }

  [Theory]
  [InlineData("You have 1 credit", "You have 1 credit%s%")]
  [InlineData("You have \a1 credit", "You have \a1 credit%s%")]
  [InlineData("You have 1 credit.", "You have 1 credit%s%.")]
  [InlineData("You have too many credits", "You have too many credits%s%")]
  public void
    StaticLocalizer_WithPercentPlurals_HandlesSingles(string translated,
      string source) {
    Assert.Equal(translated, StringLocalizer.HandlePluralization(source));
  }

  [Theory]
  [InlineData("Player MSWS' balance", "Player MSWS'%s% balance")]
  [InlineData("Player George's balance", "Player George'%s% balance")]
  [InlineData("Player MSWS' balance exceeds George's",
    "Player MSWS'%s% balance exceeds George'%s%")]
  [InlineData("MSWS' balance == MSWS' balance",
    "MSWS'%s% balance == MSWS'%s% balance")]
  public void
    StaticLocalizer_WithPercentPlurals_HandlesApostrophes(string translated,
      string source) {
    Assert.Equal(translated, StringLocalizer.HandlePluralization(source));
  }

  [Theory]
  [InlineData(2)]
  [InlineData(5)]
  [InlineData(0)]
  [InlineData(-1)]
  [InlineData(-5)]
  public void Localizer_WithPercentPlurals_HandlesMulti(int amo) {
    var currencyName = localizer.Get(MSG.CURRENCY);
    Assert.EndsWith(currencyName + "s.",
      localizer.Get(MSG.COMMAND_BALANCE, amo));
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
  public void Parity_Brackets(string _, string val) {
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
  public void Parity_Percents(string _, string val) {
    // For each opening bracket, make sure there is a closing bracket
    var percs = val.Count(c => c == '%');
    Assert.Equal(0, percs % 2);
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
    if (!key.Contains(' ')) return;
    Assert.Matches(@"[.,!? ]$", val);
  }

  [GeneratedRegex("{(.*?)}")]
  private static partial Regex ColorBrackets();
}