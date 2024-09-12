using GangsAPI.Data;
using GangsAPI.Extensions;

namespace GangsTest;

public class TestUtil {
  private static readonly Random Random = new();

  public const string FakePlayerName = "Test Player";

  public static PlayerWrapper CreateFakePlayer() {
    return new(Random.NextULong(), FakePlayerName);
  }
}