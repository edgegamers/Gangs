using GangsAPI.Data.Gang;

namespace GangsAPI.Perks;

public interface IDisplaySetting {
  Task<bool> IsChatEnabled(ulong steam);
  Task<bool> IsScoreboardEnabled(ulong steam);

  Task<bool> IsChatEnabled(IGangPlayer player) => IsChatEnabled(player.Steam);

  Task<bool> IsScoreboardEnabled(IGangPlayer player)
    => IsScoreboardEnabled(player.Steam);

  Task SetChatEnabled(ulong steam, bool enabled);
  Task SetScoreboardEnabled(ulong steam, bool enabled);
}