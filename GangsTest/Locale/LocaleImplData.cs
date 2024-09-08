using GangsAPI;
using GangsImpl;

namespace GangsTest.Locale;

public class LocaleImplData : TheoryData<string> {
  public LocaleImplData() {
    foreach (var key in Enum.GetValues<GangsAPI.Locale>()) { Add(key.Key()); }
  }
}