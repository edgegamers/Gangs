using GangsAPI.Services;
using Mock;

namespace GangsTest.GangTests;

public class GangMutabilityTests {
  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task GangMutability_Name(IGangManager mgr) {
    var dummy = await GangTestUtil.CreateGang(mgr);
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
    Assert.Equal((ulong)0, dummy.Owner);
    Assert.Single(dummy.Members);
    dummy.Name = "foobar";
    Assert.Equal("foobar", dummy.Name);
    dummy = await mgr.GetGang(dummy.GangId);
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task GangMutability_Update_Name(IGangManager mgr) {
    var dummy = await GangTestUtil.CreateGang(mgr);
    Assert.NotNull(dummy);
    Assert.Equal("name", dummy.Name);
    Assert.Equal((ulong)0, dummy.Owner);
    Assert.Single(dummy.Members);
    dummy.Name = "foobar";
    Assert.Equal("foobar", dummy.Name);
    await mgr.UpdateGang(dummy);
    dummy = await mgr.GetGang(dummy.GangId);
    Assert.NotNull(dummy);
    Assert.Equal("foobar", dummy.Name);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task GangMutability_Members(IGangManager mgr) {
    var dummy = await GangTestUtil.CreateGang(mgr);
    Assert.NotNull(dummy);
    Assert.Equal((ulong)0, dummy.Owner);
    Assert.Single(dummy.Members);
    dummy.Members.Add(1, new MockGangRank(1, "Member"));
    Assert.Equal(2, dummy.Members.Count);
    dummy = await mgr.GetGang(dummy.GangId);
    Assert.NotNull(dummy);
    Assert.Single(dummy.Members);
  }

  [Theory]
  [ClassData(typeof(GangManagerData))]
  public async Task GangMutability_Update_Members(IGangManager mgr) {
    var dummy = await GangTestUtil.CreateGang(mgr);
    Assert.NotNull(dummy);
    Assert.Equal((ulong)0, dummy.Owner);
    Assert.Single(dummy.Members);
    dummy.Members.Add(1, new MockGangRank(1, "Member"));
    Assert.Equal(2, dummy.Members.Count);
    await mgr.UpdateGang(dummy);
    dummy = await mgr.GetGang(dummy.GangId);
    Assert.NotNull(dummy);
    Assert.Equal(2, dummy.Members.Count);
  }
}