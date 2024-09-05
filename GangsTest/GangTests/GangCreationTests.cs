using GangsAPI.Data.Gang;
using GangsAPI.Services;
using Mock;

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
    Assert.Single(dummy.Ranks);
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
    Assert.Single(dummy.Ranks);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task Gang_Clone(IGangManager mgr) {
    var dummy = await mgr.CreateGang("foobar", 0);
    Assert.NotNull(dummy);
    var clone = dummy.Clone() as IGang;
    Assert.NotNull(clone);

    Assert.Equivalent(dummy, clone, true);

    Assert.NotSame(dummy, clone);
    Assert.NotSame(dummy.Members, clone.Members);
    Assert.NotSame(dummy.Ranks, clone.Ranks);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task Gang_CreateMultiple(IGangManager mgr) {
    var steam1 = (ulong)new Random().NextInt64();
    var steam2 = (ulong)new Random().NextInt64();
    var dummy1 = await mgr.CreateGang("foobar", steam1);
    Assert.NotNull(dummy1);
    Assert.Equal("foobar", dummy1.Name);
    Assert.Equal(steam1, dummy1.Owner);
    Assert.Single(dummy1.Members);
    var dummy2 = await mgr.CreateGang("barfoo", steam2);
    Assert.NotNull(dummy2);
    Assert.Equal("barfoo", dummy2.Name);
    Assert.Equal(steam2, dummy2.Owner);
    Assert.Single(dummy2.Members);
    Assert.NotSame(dummy1, dummy2);
    Assert.NotEqual(dummy1.GangId, dummy2.GangId);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task Gang_CreateMultipleFromGangPlayer(IGangManager mgr) {
    var player1 = await playerMgr.CreatePlayer((ulong)new Random().NextInt64());
    var player2 = await playerMgr.CreatePlayer((ulong)new Random().NextInt64());
    var dummy1  = await mgr.CreateGang("foobar", player1);
    var dummy2  = await mgr.CreateGang("barfoo", player2);
    Assert.NotNull(dummy1);
    Assert.NotNull(dummy2);
    Assert.Equal("foobar", dummy1.Name);
    Assert.Equal(player1.Steam, dummy1.Owner);
    Assert.Single(dummy1.Members);
    Assert.Equal("barfoo", dummy2.Name);
    Assert.Equal(player2.Steam, dummy2.Owner);
    Assert.Single(dummy2.Members);
    Assert.NotSame(dummy1, dummy2);
    Assert.NotEqual(dummy1.GangId, dummy2.GangId);
  }
}