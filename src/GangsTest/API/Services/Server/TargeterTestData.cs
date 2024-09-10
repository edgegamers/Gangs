using GangsAPI;
using GangsAPI.Services.Server;
using Mock;

namespace GangsTest.API.Services.Server;

public class TargeterTestData : TheoryData<IServerProvider, ITargeter> {
  private readonly IServerProvider provider = new MockServerProvider();
  public TargeterTestData() { Add(provider, new MockTargeter(provider)); }
}