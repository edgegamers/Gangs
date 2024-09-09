using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;

namespace GangsImpl;

public partial class PluginStringLocalizer(IStringLocalizer localizer)
  : IStringLocalizer {
  public LocalizedString this[string name] => getString(name);

  public LocalizedString this[string name, params object[] arguments]
    => new(name, string.Format(getString(name).Value, arguments));

  public IEnumerable<LocalizedString>
    GetAllStrings(bool includeParentCultures) {
    return localizer.GetAllStrings(includeParentCultures)
     .Select(str => getString(str.Name));
  }

  private LocalizedString getString(string name) {
    // Replace %[key]% with that key's value
    // Eg: if we have a locale key of "prefix", then
    // other locale values can use %prefix% to reference it.
    var value   = localizer[name].Value;
    var matches = percents().Matches(value);
    foreach (Match match in matches)
      // Check if the key exists
      try {
        var key = match.Groups[0].Value;
        value = value.Replace(key,
          key == "%prefix%" ?
            this[key[1..^1]].Value :
            this[key[1..^1]].Value.Trim());
      } catch (NullReferenceException) { }

    return new LocalizedString(name, value);
  }

  [GeneratedRegex("%.*?%")]
  private static partial Regex percents();
}