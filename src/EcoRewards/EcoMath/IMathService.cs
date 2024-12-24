using CounterStrikeSharp.API.Core;

namespace EcoRewards.EcoMath;

public interface IMathService {
  struct MathParams {
    public string Equation;
    public double Answer;
    public int Reward;
  }

  public MathParams? Question { get; }

  void StartMath(MathParams? mathParams = null);
  void StopMath(CCSPlayerController? winner);
}