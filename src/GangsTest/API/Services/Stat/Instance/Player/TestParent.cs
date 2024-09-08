using GangsAPI.Data.Gang;
using GangsAPI.Services.Player;

namespace GangsTest.API.Services.Stat.Instance.Player;

public class TestParent(IPlayerManager playerMgr) : StatTestParent {
  protected readonly IGangPlayer TestPlayer = playerMgr
   .CreatePlayer((ulong)new Random().NextInt64(), "Test Player")
   .GetAwaiter()
   .GetResult() ?? throw new InvalidOperationException();
}