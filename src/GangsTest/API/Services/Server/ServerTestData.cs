using GangsAPI.Services.Server;
using Mock;

namespace GangsTest.API.Services.Server;

public class ServerTestData : TheoryData<IServerProvider> {
  public ServerTestData() { Add(new MockServerProvider()); }
}