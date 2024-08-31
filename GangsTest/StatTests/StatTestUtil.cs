using GangsAPI.Data.Stat;
using GangsAPI.Services;

namespace GangsTest.StatTests;

public class StatTestUtil {
  public static async Task<IStat?> CreateStat(IStatManager statManager,
    string id = "id", string name = "name", string? desc = "desc") {
    var dummy = await statManager.CreateStat(id, name, desc);
    return dummy;
  }
}