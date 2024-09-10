using GangsAPI.Data;
using GangsAPI.Extensions;

namespace GangsTest.API.Services.Server;

public abstract class TestParent {
  protected readonly PlayerWrapper TestPlayer =
    new(new Random().NextUInt(), "Test Player");
}