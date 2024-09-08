using Microsoft.Extensions.Localization;

namespace GangsTest.TestLocale;

public class LocaleFileKVData : TheoryData<string, string> {
  public LocaleFileKVData() {
    foreach (var localizedString in StringLocalizer.Instance.GetAllStrings())
      Add(localizedString.Name, localizedString.Value);
  }
}