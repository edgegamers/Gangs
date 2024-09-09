using GangsAPI.Data.Gang;
using GangsAPI.Extensions;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;

namespace GangsTest.API.Services.Gang;

public class CreationTests(IPlayerManager playerMgr) {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Basic(IGangManager mgr) {
    var dummy = await mgr.CreateGang("foobar", new Random().NextUInt());
    Assert.NotNull(dummy);
    Assert.Equal("foobar", dummy.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Create_Delete(IGangManager mgr) {
    var dummy = await mgr.CreateGang("foobar", new Random().NextUInt());
    Assert.NotNull(dummy);
    Assert.Equal("foobar", dummy.Name);
    await mgr.DeleteGang(dummy.GangId);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task From_Player(IGangManager mgr) {
    var player = await playerMgr.CreatePlayer(0);
    var dummy  = await mgr.CreateGang("foobar", player);
    Assert.NotNull(dummy);
    Assert.Equal("foobar", dummy.Name);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Already_In_Gang(IGangManager mgr) {
    Assert.NotNull(await mgr.CreateGang("foobar", 0));
    await Assert.ThrowsAnyAsync<InvalidOperationException>(async ()
      => await mgr.CreateGang("barfoo", 0));
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Clone(IGangManager mgr) {
    var dummy = await mgr.CreateGang("foobar", 0);
    Assert.NotNull(dummy);
    var clone = dummy.Clone() as IGang;
    Assert.NotNull(clone);

    Assert.Equivalent(dummy, clone, true);

    Assert.NotSame(dummy, clone);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Multiple(IGangManager mgr) {
    var steam1 = (ulong)new Random().NextInt64();
    var steam2 = (ulong)new Random().NextInt64();
    var dummy1 = await mgr.CreateGang("foobar", steam1);
    Assert.NotNull(dummy1);
    Assert.Equal("foobar", dummy1.Name);
    var dummy2 = await mgr.CreateGang("barfoo", steam2);
    Assert.NotNull(dummy2);
    Assert.Equal("barfoo", dummy2.Name);
    Assert.NotSame(dummy1, dummy2);
    Assert.NotEqual(dummy1.GangId, dummy2.GangId);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task From_Player_Multiple(IGangManager mgr) {
    var player1 = await playerMgr.CreatePlayer((ulong)new Random().NextInt64());
    var player2 = await playerMgr.CreatePlayer((ulong)new Random().NextInt64());
    var dummy1  = await mgr.CreateGang("foobar", player1);
    var dummy2  = await mgr.CreateGang("barfoo", player2);
    Assert.NotNull(dummy1);
    Assert.NotNull(dummy2);
    Assert.Equal("foobar", dummy1.Name);
    Assert.Equal("barfoo", dummy2.Name);
    Assert.NotSame(dummy1, dummy2);
    Assert.NotEqual(dummy1.GangId, dummy2.GangId);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Injection_Test(IGangManager mgr) {
    string[] strings = ["\"\"", "'' OR 1=1 --", "'; DROP TABLE users; --"];
    foreach (var str in strings) {
      var dummy = await mgr.CreateGang(str, new Random().NextUInt());
      Assert.NotNull(dummy);
      Assert.Equal(str, dummy.Name);
      await mgr.DeleteGang(dummy.GangId);
    }
  }
}