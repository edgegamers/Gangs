using System.Collections;
using Microsoft.Extensions.Localization;

namespace GangsTest.TestLocale;

public class LocaleFileKVData : TheoryData<string, string> {
  public LocaleFileKVData() {
    Add("prefix", "Example Prefix");
    foreach (var key in StringLocalizer.Instance.GetAllStrings(true))
      Add(key.Name, key.Value);
  }
}