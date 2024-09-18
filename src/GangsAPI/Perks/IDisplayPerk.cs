using GangsAPI.Data.Gang;

namespace GangsAPI.Perks;

public interface IDisplayPerk : IPerk {
  int ChatCost { get; }
  int ScoreboardCost { get; }
  Task<bool> HasChatDisplay(IGang gang);
  Task<bool> HasScoreboardDisplay(IGang gang);

  Task SetChatDisplay(IGang gang, bool value);
  Task SetScoreboardDisplay(IGang gang, bool value);
}