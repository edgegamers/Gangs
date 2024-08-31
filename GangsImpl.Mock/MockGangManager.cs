using GangsAPI.Data.Gang;
using GangsAPI.Services;

namespace GangsImpl.Memory;

public class MockGangManager : IGangManager {
  private readonly HashSet<IGang> gangs = [];

  public Task<IEnumerable<IGang>> GetGangs() {
    return Task.FromResult(gangs.AsEnumerable());
  }

  public Task<IGang?> GetGang(int id) {
    return Task.FromResult(gangs.FirstOrDefault(g => g.GangId == id));
  }

  public Task<IGang?> GetGang(ulong steam) {
    return Task.FromResult(
      gangs.FirstOrDefault(g => g.Members.ContainsKey(steam)));
  }

  public Task<bool> UpdateGang(IGang gang) {
    var g = gangs.FirstOrDefault(g => g.GangId == gang.GangId);
    if (g == null) return Task.FromResult(false);
    g.Name = gang.Name;
    g.Members.Clear();
    foreach (var member in gang.Members) g.Members.Add(member);
    return Task.FromResult(true);
  }

  public Task<bool> DeleteGang(int id) {
    return Task.FromResult(gangs.RemoveWhere(g => g.GangId == id) > 0);
  }

  public Task<IGang?> CreateGang(string name, ulong owner) {
    var id   = gangs.Count + 1;
    var gang = new MockGang(id, name, owner);
    return Task.FromResult((IGang?)(gangs.Add(gang) ? gang.Clone() : null));
  }
}