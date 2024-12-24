using GangsAPI.Data;
using GangsAPI.Data.Gang;

namespace GangsAPI.Services.Server;

public interface IGangTargeter {
  Task<IGang?> FindGang(string query, PlayerWrapper? executor = null, bool print = true);
  Task<IGang?> FindGangOrByMember(string query, PlayerWrapper? executor = null, bool print = true);
}