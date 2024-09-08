using CounterStrikeSharp.API.Core.Translations;
using Microsoft.Extensions.Localization;

namespace GangsTest.TestLocale;

public class LocalFileLocalizerFactory : IStringLocalizerFactory {
  private const string path = @"..\..\..\..\Gangs\lang";

  public IStringLocalizer Create(Type resourceSource) {
    return new JsonStringLocalizer(path);
  }

  public IStringLocalizer Create(string baseName, string location) {
    return new JsonStringLocalizer(path);
  }
}