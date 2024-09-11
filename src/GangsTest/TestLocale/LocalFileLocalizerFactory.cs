using System.Diagnostics;
using CounterStrikeSharp.API.Core.Translations;
using Microsoft.Extensions.Localization;

namespace GangsTest.TestLocale;

public class LocalFileLocalizerFactory : IStringLocalizerFactory {
  private readonly string path;

  public LocalFileLocalizerFactory() {
    // Lang folder is in the root of the project
    // keep moving up until we find it
    var current = Directory.GetCurrentDirectory();
    while (!Directory.Exists(Path.Combine(current, "lang"))) {
      current = Directory.GetParent(current)?.FullName;
      if (current == null)
        throw new DirectoryNotFoundException("Could not find lang folder");
    }

    path = Path.Combine(current, "lang");
  }

  public IStringLocalizer Create(Type resourceSource) {
    return new JsonStringLocalizer(path);
  }

  public IStringLocalizer Create(string baseName, string location) {
    return new JsonStringLocalizer(path);
  }
}