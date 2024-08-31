using GangsAPI.Data.Gang;
using GangsAPI.Services;

namespace GangsTest.GangTests;

public class GangTestUtil {
  public static async Task<IGang?> CreateGang(IGangManager gangManager,
    string name = "name", ulong owner = 0) {
    var dummy = await gangManager.CreateGang(name, owner);
    return dummy;
  }
}