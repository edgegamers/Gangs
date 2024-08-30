using GangsAPI.Extensions;
using GangsAPI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GangsImpl.Memory;

public static class MemoryImpl {
  public static void AddMemoryImpl(this IServiceCollection collection) {
    collection.AddPluginBehavior<IStatManager, MemoryStatManager>();
  }
}