﻿using GangsAPI.Data.Gang;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;

namespace GangsTest.API.Services.Gang;

public class FetchTests(IPlayerManager players) {
  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Gang_Create(IGangManager mgr) {
    Assert.Empty(await mgr.GetGangs());
    var dummy = await mgr.CreateGang("foobar", 0);
    var gangs = (await mgr.GetGangs()).ToHashSet();
    Assert.NotNull(dummy);
    Assert.NotNull(gangs);
    Assert.Single(gangs);
    Assert.Equal(dummy, gangs.First());
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Gang_CreateFromGangPlayer(IGangManager mgr) {
    Assert.Empty(await mgr.GetGangs());
    var player = await players.CreatePlayer(0);
    var dummy  = await mgr.CreateGang("foobar", player);
    var gangs  = (await mgr.GetGangs()).ToHashSet();
    Assert.NotNull(dummy);
    Assert.NotNull(gangs);
    Assert.Single(gangs);
    Assert.Equal(dummy, gangs.First());
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Gang_Clone(IGangManager mgr) {
    var dummy = await mgr.CreateGang("foobar", 0);
    Assert.NotNull(dummy);
    var clone = dummy.Clone() as IGang;
    Assert.NotNull(clone);
    Assert.NotSame(dummy, clone);
    Assert.Equal(dummy, clone);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Gang_CreateMultiple(IGangManager mgr) {
    var dummy1 = await mgr.CreateGang("foobar", 0);
    Assert.NotNull(dummy1);
    var dummy2 = await mgr.CreateGang("barfoo", 1);
    Assert.NotNull(dummy2);
    Assert.NotSame(dummy1, dummy2);
    var gangs = (await mgr.GetGangs()).ToHashSet();
    Assert.NotNull(gangs);
    Assert.Equal(2, gangs.Count);
    Assert.Contains(gangs, g => g.GangId == dummy1.GangId);
    Assert.Contains(gangs, g => g.GangId == dummy2.GangId);
  }

  [Theory]
  [ClassData(typeof(TestData))]
  public async Task Gang_FetchId(IGangManager mgr) {
    var dummy = await mgr.CreateGang("foobar", (ulong)new Random().NextInt64());
    Assert.NotNull(dummy);
    var gang = await mgr.GetGang(dummy.GangId);
    Assert.NotNull(gang);
    Assert.Equal(dummy, gang);
  }
}