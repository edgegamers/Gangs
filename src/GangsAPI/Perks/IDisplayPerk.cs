using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;

namespace GangsAPI.Perks;

public interface IDisplayPerk : IPerk {
  Task<bool> HasChatDisplay(IGang gang);
  Task<bool> HasScoreboardDisplay(IGang gang);
  
  Task SetChatDisplay(IGang gang, bool value);
  Task SetScoreboardDisplay(IGang gang, bool value);
  
  int ChatCost { get; }
  int ScoreboardCost { get; }
}