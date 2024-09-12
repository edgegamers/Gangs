using GangsAPI.Data;
using GangsAPI.Extensions;

namespace GangsTest;

public class TestUtil {
  public const string FakePlayerName = "Test Player";
  private static readonly Random Random = new();

  public static PlayerWrapper CreateFakePlayer() {
    return new PlayerWrapper(Random.NextULong(), FakePlayerName);
  }
}