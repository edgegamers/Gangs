using Microsoft.Extensions.Localization;

namespace GangsTest.Locale;

public class LocaleKeyValueData : TheoryData<string, string> {
  public LocaleKeyValueData() {
    foreach (var localizedString in StringLocalizer.Instance.GetAllStrings())
      Add(localizedString.Name, localizedString);
  }
}