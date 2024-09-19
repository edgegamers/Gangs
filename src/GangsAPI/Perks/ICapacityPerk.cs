using GangsAPI.Data.Gang;

namespace GangsAPI.Perks;

public interface ICapacityPerk : IPerk {
  int MaxCapacity { get; }
  Task<int> GetCapacity(int gangid);

  Task<int> GetCapacity(IGangPlayer player) {
    return player.GangId == null ?
      Task.FromResult(0) :
      GetCapacity(player.GangId.Value);
  }

  Task<int> GetCapacity(IGang gang) { return GetCapacity(gang.GangId); }
}