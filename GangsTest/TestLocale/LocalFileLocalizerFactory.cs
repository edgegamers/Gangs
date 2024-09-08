using CounterStrikeSharp.API.Core.Translations;
using Microsoft.Extensions.Localization;

namespace GangsTest.TestLocale;

public class LocalFileLocalizerFactory : IStringLocalizerFactory {
  private readonly string path;

  public LocalFileLocalizerFactory() {
    var current = Directory.GetCurrentDirectory().Split("GangsTest")[^2];
    var parent  = Directory.GetParent(current);
    if (parent is null)
      throw new DirectoryNotFoundException("Could not find parent directory");
    path = Path.Combine(parent.FullName, "Gangs", "lang");
    Console.WriteLine(path);
  }

  public IStringLocalizer Create(Type resourceSource) {
    return new JsonStringLocalizer(path);
  }

  public IStringLocalizer Create(string baseName, string location) {
    return new JsonStringLocalizer(path);
  }
}