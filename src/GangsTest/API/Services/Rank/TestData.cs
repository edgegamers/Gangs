using GangsAPI.Services;
using Mock;

namespace GangsTest.API.Services.Rank;

public class TestData : TheoryData<IRankManager> {
  public TestData() { Add(new MockRankManager()); }
}