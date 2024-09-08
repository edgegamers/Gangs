using GangsAPI;

namespace GangsTest.TestLocale;

public class LocaleImplData : TheoryData<string> {
  public LocaleImplData() {
    foreach (var key in Enum.GetValues<GangsAPI.MSG>()) { Add(key.Key()); }
  }
}