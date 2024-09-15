using GangsAPI.Data.Gang;

namespace GangsAPI.Perks;

public interface IMotdPerk {
  Task<string?> GetMotd(int gangid);

  Task<string?> GetMotd(IGangPlayer player) {
    return player.GangId == null ?
      Task.FromResult<string?>(null) :
      GetMotd(player.GangId.Value);
  }

  Task<string?> GetMotd(IGang gang) { return GetMotd(gang.GangId); }
}