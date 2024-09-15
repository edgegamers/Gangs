using GangsAPI.Services.Server;
using Mock;

namespace GangsTest.API.Services.Server;

public class TargeterTestData : TheoryData<IServerProvider, IPlayerTargeter> {
  private readonly IServerProvider provider = new MockServerProvider();
  public TargeterTestData() { Add(provider, new MockPlayerTargeter(provider)); }
}