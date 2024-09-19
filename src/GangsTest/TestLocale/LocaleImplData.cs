using GangsAPI;

namespace GangsTest.TestLocale;

public class LocaleImplData : TheoryData<string> {
  public LocaleImplData() {
    foreach (var key in Enum.GetValues<MSG>()) Add(key.Key());
  }
}