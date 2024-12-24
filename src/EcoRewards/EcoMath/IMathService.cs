using CounterStrikeSharp.API.Core;

namespace EcoRewards.EcoMath;

public interface IMathService {
  public MathParams? Question { get; }

  void StartMath(MathParams? mathParams = null);
  void StopMath(CCSPlayerController? winner);

  struct MathParams {
    public string Equation;
    public double Answer;
    public int Reward;
  }
}