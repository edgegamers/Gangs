using CounterStrikeSharp.API.Core.Translations;
using Microsoft.Extensions.Localization;

namespace GangsTest.TestLocale;

public class LocalFileLocalizerFactory : IStringLocalizerFactory {
  private readonly string path = Path.Combine(
    Directory.GetCurrentDirectory().Split("Gangs")[0], "Gangs", "Gangs",
    "lang");

  public IStringLocalizer Create(Type resourceSource) {
    return new JsonStringLocalizer(path);
  }

  public IStringLocalizer Create(string baseName, string location) {
    return new JsonStringLocalizer(path);
  }
}