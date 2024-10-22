namespace Raffle;

public class Raffle(int buyIn) {
  private readonly HashSet<ulong> players = [];

  public int BuyIn => buyIn;

  public int Value => players.Count * buyIn;

  public int TotalPlayers => players.Count;

  public void AddPlayer(ulong player) { players.Add(player); }

  /// <summary>
  ///   Get the winner of the raffle
  /// </summary>
  /// <returns>The winner, or null if there are no players</returns>
  public ulong? GetWinner() {
    if (players.Count == 0) return null;
    var random      = new Random();
    var winnerIndex = random.Next(players.Count);
    // remove the winner from the list of players
    var winner = players.ElementAt(winnerIndex);
    players.Remove(winner);
    return winner;
  }
}