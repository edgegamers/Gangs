using GangsAPI.Services;

namespace GangsTest.GangTests;

public class GangCreationTests(IPlayerManager playerMgr) {
  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task Gang_Create(IGangManager mgr) {
    var dummy = await mgr.CreateGang("foobar", 0);
    Assert.NotNull(dummy);
    Assert.Equal("foobar", dummy.Name);
    Assert.Equal((ulong)0, dummy.Owner);
    Assert.Single(dummy.Members);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task Gang_CreateFromGangPlayer(IGangManager mgr) {
    var player = await playerMgr.CreatePlayer(0);
    var dummy  = await mgr.CreateGang("foobar", player);
    Assert.NotNull(dummy);
    Assert.Equal("foobar", dummy.Name);
    Assert.Equal((ulong)0, dummy.Owner);
    Assert.Single(dummy.Members);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task Gang_CreateMultiple(IGangManager mgr) {
    var dummy1 = await mgr.CreateGang("foobar", 0);
    Assert.NotNull(dummy1);
    Assert.Equal("foobar", dummy1.Name);
    Assert.Equal((ulong)0, dummy1.Owner);
    Assert.Single(dummy1.Members);
    var dummy2 = await mgr.CreateGang("barfoo", 0);
    Assert.NotNull(dummy2);
    Assert.Equal("barfoo", dummy2.Name);
    Assert.Equal((ulong)0, dummy2.Owner);
    Assert.Single(dummy2.Members);
    Assert.NotSame(dummy1, dummy2);
    Assert.NotEqual(dummy1.GangId, dummy2.GangId);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task Gang_CreateMultipleFromGangPlayer(IGangManager mgr) {
    var player = await playerMgr.CreatePlayer(0);
    var dummy  = await mgr.CreateGang("foobar", player);
    Assert.NotNull(dummy);
    Assert.Equal("foobar", dummy.Name);
    Assert.Equal((ulong)0, dummy.Owner);
    Assert.Single(dummy.Members);
  }
}