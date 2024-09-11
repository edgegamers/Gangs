using GangsAPI.Data.Gang;

namespace GangsAPI.Perks;

public interface IGangChatPerk : IPerk<bool> {
  Task SendGangChat(IGangPlayer player, IGang gang, string message);
}