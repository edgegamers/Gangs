using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;

namespace GangsTest.TestLocale;

public partial class StringLocalizer : IStringLocalizer {
  public static StringLocalizer Instance
    => new(new LocalFileLocalizerFactory());

  private readonly IStringLocalizer localizer;

  public StringLocalizer(IStringLocalizerFactory factory) {
    var type = typeof(StringLocalizer);
    var assemblyName =
      new AssemblyName(type.GetTypeInfo().Assembly.FullName ?? string.Empty);
    localizer = factory.Create(string.Empty, assemblyName.FullName);
  }

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