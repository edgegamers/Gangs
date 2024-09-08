using GangsAPI;
using Microsoft.Extensions.Localization;

namespace GangsTest.TestLocale;

public class ParityTests(IStringLocalizer localizer) {
  [Theory]
  [ClassData(typeof(LocaleImplData))]
  public void Localize_Exists_FromImpl(string key) {
    Assert.False(string.IsNullOrEmpty(localizer[key]));
  }

  [Theory]
  [ClassData(typeof(LocaleFileKVData))]
  public void Localize_Exists_FromFile(string key, string _) {
    // Make sure we have an enum whose key is the same as the key in the file
    Assert.Contains(key, localeKeys);
  }

  private readonly IEnumerable<string> localeKeys =
    Enum.GetValues<GangsAPI.MSG>().Select(s => s.Key());
}