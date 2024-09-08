using System.Collections;
using Microsoft.Extensions.Localization;

namespace GangsTest.TestLocale;

public class LocaleFileKVData : IEnumerable<object[]> {
  public IEnumerator<object[]> GetEnumerator() {
    return StringLocalizer.Instance.GetAllStrings(true)
     .Select(keyValuePair => (object[]) [keyValuePair.Name, keyValuePair.Value])
     .GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}