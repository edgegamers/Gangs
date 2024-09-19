using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Localization;

namespace GangsTest.TestLocale;

public partial class StringLocalizer : IStringLocalizer {
  public static readonly StringLocalizer Instance =
    new(new LocalFileLocalizerFactory());

  private readonly IStringLocalizer localizer;

  public StringLocalizer(IStringLocalizerFactory factory) {
    var type = typeof(StringLocalizer);
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
    var value = localizer[name].Value;

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
        var replacement = getString(trimmedKey).Value;
        value = value.Replace(key,
          trimmedKey == "prefix" ? replacement : replacement.Trim());
      } catch (NullReferenceException) {
        // Key doesn't exist, move on
      }
    }

    // Format with arguments if provided
    if (arguments.Length > 0) value = string.Format(value, arguments);

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

    var trailingIndex = -1;

    // We have to do this chicanery due to supporting colors in the string

    while ((trailingIndex =
      value.IndexOf("'s", trailingIndex + 1, StringComparison.Ordinal)) != -1) {
      var startingWordBoundary = value[..trailingIndex].LastIndexOf(' ');
      var endingWordBoundary = value.IndexOf(' ', trailingIndex + 2);
      var word = value[(startingWordBoundary + 1)..endingWordBoundary];
      var filteredWord = word.Where(c => char.IsLetterOrDigit(c) || c == '\'')
       .ToArray();
      if (new string(filteredWord).EndsWith("s's",
        StringComparison.OrdinalIgnoreCase))
        value = value[..(trailingIndex + 1)] + " "
          + value[(trailingIndex + 4)..];
    }

    return value;
  }
}