namespace GangsAPI.Extensions;

public static class RandomExtensions {
  public static uint NextUInt(this Random random) {
    return (uint)random.NextInt64();
  }
}