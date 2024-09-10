using GangsAPI.Data.Gang;
using GangsAPI.Services.Gang;

namespace GangsTest.API.Services.Stat.Instance.Gang;

public class TestParent(IGangManager gangs) : StatTestParent {
  protected readonly IGang TestGang = gangs
   .CreateGang("Test Gang", (ulong)new Random().NextInt64())
   .GetAwaiter()
   .GetResult() ?? throw new InvalidOperationException();
}