using GangsAPI.Data.Stat;
using GangsAPI.Services;
using GangsTest.GangTests;

namespace GangsTest.GangBankTests;

public class GangBankTest {
  private readonly IStat bankStat = new MockBankStat();
  private readonly IGangManager gangManager;
  private readonly IGangStatManager gangStatManager;
  private readonly IStatManager statManager;

  public GangBankTest(IGangManager gangManager, IStatManager statManager,
    IGangStatManager gangStatManager) {
    this.gangManager     = gangManager;
    this.statManager     = statManager;
    this.gangStatManager = gangStatManager;

    Assert.True(
      this.statManager.RegisterStat(bankStat).GetAwaiter().GetResult());
    Assert.NotNull(this.statManager.GetStat(bankStat.StatId)
     .GetAwaiter()
     .GetResult());
  }

  [Fact]
  public async Task GangBank_ZeroBalance() {
    var gang = await GangTestUtil.CreateGang(gangManager);
    Assert.NotNull(gang);
    Assert.Null(gang.Bank);
    gang.Stats.Add(bankStat);
    Assert.Equal(gang.Bank, 0);

    Assert.True(await gangManager.UpdateGang(gang));
    gang = await gangManager.GetGang(gang.GangId);
    Assert.NotNull(gang);
    Assert.Null(gang.Bank);
  }

  [Fact]
  public async Task GangBank_Deposit() {
    var gang = await GangTestUtil.CreateGang(gangManager);
    Assert.NotNull(gang);
    gang.Stats.Add(bankStat);
    Assert.Equal(gang.Bank, 0);

    var stat = gang.GetStat(bankStat.StatId);
    Assert.Same(stat, bankStat);

    var bank = stat as IGangStat<int>;
    Assert.NotNull(bank);
    Assert.Equal(0, bank.Value);

    bank.Value += 1;
    Assert.NotEmpty(await statManager.GetStats());
    Assert.True(await gangStatManager.PushToGang(gang, bank),
      "Failed to push bank value to gang");
    gang = await gangManager.GetGang(gang.GangId);
    Assert.NotNull(gang);

    bank = await gangStatManager.GetForGang<int>(gang, bank);
    Assert.NotNull(bank);
    Assert.Equal(1, bank.Value);
  }

  [Theory]
  [InlineData(50, 25)]
  [InlineData(5, 25)]
  [InlineData(-5, 25)]
  [InlineData(-5, -25)]
  public async Task GangBank_Withdraw(int initial, int withdraw) {
    var gang = await GangTestUtil.CreateGang(gangManager);
    Assert.NotNull(gang);
    Assert.True(await gangStatManager.PushToGang(gang, bankStat, initial));
    var stat = await gangStatManager.GetForGang<int>(gang, bankStat);
    Assert.NotNull(stat);
    Assert.Equal(initial, stat.Value);

    stat.Value -= withdraw;
    Assert.True(await gangStatManager.PushToGang(gang, stat));
    stat = await gangStatManager.GetForGang<int>(gang, bankStat);
    Assert.NotNull(stat);
    Assert.Equal(initial - withdraw, stat.Value);
  }

  [Theory]
  [InlineData(50, 25)]
  [InlineData(5, 25)]
  [InlineData(-5, 25)]
  [InlineData(-5, -25)]
  public async Task
    GangBank_Withdraw_Indirect_Alias(int initial, int withdraw) {
    var gang = await GangTestUtil.CreateGang(gangManager);
    Assert.NotNull(gang);
    Assert.True(await gangStatManager.PushToGang(gang, bankStat, initial));
    var stat = await gangStatManager.GetForGang<int>(gang, bankStat);
    Assert.NotNull(stat);
    Assert.Equal(initial, stat.Value);

    gang.Stats.Add(stat);
    gang.Bank -= withdraw;

    Assert.True(await gangStatManager.PushToGang(gang, stat));
    stat = await gangStatManager.GetForGang<int>(gang, bankStat);
    Assert.NotNull(stat);
    Assert.Equal(initial - withdraw, stat.Value);
    Assert.Equal(initial - withdraw, gang.Bank);
  }
}