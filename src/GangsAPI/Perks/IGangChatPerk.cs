using GangsAPI.Data.Gang;

namespace GangsAPI.Perks;

public interface IGangChatPerk : IPerk {
  Task SendGangChat(IGangPlayer player, IGang gang, string message) {
    return SendGangChat(player.Name ?? player.Steam.ToString(), gang, message);
  }

  Task SendGangChat(string name, IGang gang, string message);

  Task SendGangChat(IGang gang, string message) {
    return SendGangChat("SYSTEM", gang, message);
  }

  void ClearChatHistory(IGang gang);

  IEnumerable<string> GetChatHistory(IGang gang);
}