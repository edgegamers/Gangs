using GangsAPI.Data.Gang;
using Mock;

namespace GangsTest.API.Services.Rank;

public abstract class TestParent {
  protected readonly IGang TestGang =
    new MockGang(new Random().Next(), "Test Gang");

  protected int GangId => TestGang.GangId;
}