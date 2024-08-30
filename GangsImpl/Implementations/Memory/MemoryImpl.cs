using GangsAPI.Services;
using GangsImpl.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GangsImpl.Implementations.Memory;

public static class MemoryImpl {
  public static void AddFlatFileImpl(this IServiceCollection collection) {
    collection.AddPluginBehavior<IStatManager, StatManager>();
  }
}