using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;

namespace GangsImpl;

public partial class PluginStringLocalizer : IStringLocalizer {
  private readonly IStringLocalizer localizer;

  public PluginStringLocalizer(IStringLocalizerFactory factory) {
    var type = typeof(PluginStringLocalizer);
    var assemblyName =
      new AssemblyName(type.GetTypeInfo().Assembly.FullName ?? string.Empty);
    localizer = factory.Create(string.Empty, assemblyName.FullName);
  }

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
        // CS# forces a space before a chat color if the entireity
        // of the strong is a color code. This is undesired
        // in our case, so we trim the value when we have a prefix.
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