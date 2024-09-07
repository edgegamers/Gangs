using GangsAPI.Extensions;
using GangsAPI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mock;

public static class MemoryImpl {
  public static void AddMemoryImpl(this IServiceCollection collection) {
    collection.AddPluginBehavior<IStatManager, Creation>();
  }
}