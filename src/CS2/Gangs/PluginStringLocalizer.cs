using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;

namespace GangsImpl;

public partial class PluginStringLocalizer(IStringLocalizer localizer)
  : IStringLocalizer {
  public LocalizedString this[string name] => GetString(name);

  public LocalizedString this[string name, params object[] arguments]
    => new(name, string.Format(GetString(name).Value, arguments));

  public IEnumerable<LocalizedString>
    GetAllStrings(bool includeParentCultures) {
    return localizer.GetAllStrings(includeParentCultures)
     .Select(str => GetString(str.Name));
  }

  private LocalizedString GetString(string name) {
    // Replace %[key]% with that key's value
    // Eg: if we have a locale key of "prefix", then
    // other locale values can use %prefix% to reference it.
    var value   = localizer[name].Value;
    var matches = Percents().Matches(value);
    foreach (Match match in matches)
      // Check if the key exists
      try {
        var key = match.Groups[0].Value;
        value = value.Replace(key, localizer[key[1..^1]].Value);
      } catch (NullReferenceException) { }

    return new LocalizedString(name, value);
  }

  [GeneratedRegex(@"%.*?%")]
  private static partial Regex Percents();
}