namespace Raffle;

public interface IRaffleManager {
  Raffle? Raffle { get; }

  bool StartRaffle(int buyIn);
  bool AreEntriesOpen();
  void SetEntriesOpen(float seconds);
  void DrawWinner();
}