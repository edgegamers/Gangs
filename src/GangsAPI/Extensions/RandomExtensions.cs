namespace GangsAPI.Extensions;

public static class RandomExtensions {
  public static ulong NextULong(this Random random) {
    return (ulong)random.NextInt64();
  }
}