using GangsAPI.Services;
using GangsAPI.Struct.Stat;

namespace GangsTest;

public class StatTestUtil {
  public static async Task<IStat?> CreateStat(IStatManager statManager,
    string id = "id", string name = "name", string? desc = "desc") {
    var dummy = await statManager.CreateStat(id, name, desc);
    return dummy;
  }
}