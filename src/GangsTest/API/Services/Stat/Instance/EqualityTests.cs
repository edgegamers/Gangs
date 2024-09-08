using Mock;

namespace GangsTest.API.Services.Stat.Instance;

public class EqualityTests {
  [Fact]
  public void Stat_Equality_SameEverything() {
    var foo1 = new MockStat<int>("foo", "bar", null, 0);
    var foo2 = new MockStat<int>("foo", "bar", null, 0);
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Fact]
  public void Stat_Equality_DiffName() {
    var foo1 = new MockStat<int>("foo", "foobar", null, 0);
    var foo2 = new MockStat<int>("foo", "bar", null, 0);
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Fact]
  public void Stat_Equality_DiffDesc() {
    var foo1 = new MockStat<int>("foo", "bar", null, 0);
    var foo2 = new MockStat<int>("foo", "bar", "foobar", 0);
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Fact]
  public void Stat_Equality_DiffBoth() {
    var foo1 = new MockStat<int>("foo", "barfoo", null, 0);
    var foo2 = new MockStat<int>("foo", "bar", "foobar", 0);
    Assert.Equal(foo1, foo2);
    Assert.StrictEqual(foo1, foo2);
  }

  [Fact]
  public void Stat_Equality_DiffIDs() {
    var foo1 = new MockStat<int>("foo", "bar", null, 0);
    var foo2 = new MockStat<int>("foobar", "bar", null, 0);
    Assert.NotStrictEqual(foo1, foo2);
    Assert.NotEqual(foo1, foo2);
  }
}