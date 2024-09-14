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
    => getString(name, arguments);

  public IEnumerable<LocalizedString>
    GetAllStrings(bool includeParentCultures) {
    return localizer.GetAllStrings(includeParentCultures)
     .Select(str => getString(str.Name));
  }

  [GeneratedRegex("%.*?%")]
  private static partial Regex percents();

  [GeneratedRegex(@"\b(\w+)%s%")]
  private static partial Regex plural();

  private LocalizedString getString(string name, params object[] arguments) {
    // Get the localized value
    var value = localizer[name, arguments].Value;

    // Replace placeholders like %key% with their respective values
    var matches = percents().Matches(value);
    foreach (Match match in matches) {
      var key        = match.Value;
      var trimmedKey = key[1..^1]; // Trim % symbols

      // NullReferenceException catch block if key does not exist
      try {
        // CS# forces a space before a chat color if the entireity
        // of the strong is a color code. This is undesired
        // in our case, so we trim the value when we have a prefix.
        if (trimmedKey == "s") continue;
        var replacement = getString(trimmedKey).Value;
        value = value.Replace(key,
          trimmedKey == "prefix" ? replacement : replacement.Trim());
      } catch (NullReferenceException) {
        // Key doesn't exist, move on
      }
    }

    // Handle pluralization
    value = HandlePluralization(value);
    return new LocalizedString(name, value);
  }

  public static string HandlePluralization(string value) {
    var pluralMatches = plural().Matches(value);
    foreach (Match match in pluralMatches) {
      var word   = match.Groups[1].Value.ToLower();
      var index  = match.Index;
      var prefix = value[..index].Trim();

      var lastWords = prefix.Split(' ')
       .Select(w
          => w.Where(c => char.IsLetterOrDigit(c) || c == '-').ToArray());

      var previousNumber = lastWords.LastOrDefault(w => int.TryParse(w, out _));

      if (previousNumber != null)
        value = value[..index] + value[index..]
         .Replace("%s%", int.Parse(previousNumber) == 1 ? "" : "s");
      else
        value = value[..index] + value[index..]
         .Replace("%s%", word.EndsWith('s') ? "" : "s");
    }

    value = value.Replace("%s%", "s");
    while (value.Contains("s's", StringComparison.CurrentCultureIgnoreCase)) {
      var aposIndex = value.IndexOf("s's", StringComparison.OrdinalIgnoreCase);
      value = value[..(aposIndex + 1)] + "' " + value[(aposIndex + 4)..];
    }

    return value;
  }
}