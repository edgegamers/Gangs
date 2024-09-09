using GangsAPI.Data.Gang;
using GangsAPI.Extensions;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using Mock;

namespace GangsTest.API.Services.Rank;

public abstract class TestParent() {
  protected readonly IGang TestGang =
    new MockGang(new Random().Next(), "Test Gang");

  protected int GangId => TestGang.GangId;
}