using System.Data;
using NCalc;

namespace EcoRewards.EcoMath;

public class EquationBuilder {
  public string Equation { get; private set; }

  public EquationBuilder(int start) { Equation = start.ToString(); }

  public EquationBuilder WithAddition(int x, bool parentheses = false) {
    if (parentheses && Equation.Contains(' '))
      Equation = $"({Equation}) + {x}";
    else
      Equation += $" + {x}";
    return this;
  }

  public EquationBuilder WithSubtraction(int x, bool parentheses = false) {
    if (parentheses && Equation.Contains(' '))
      Equation = $"({Equation}) - {x}";
    else
      Equation += $" - {x}";
    return this;
  }

  public EquationBuilder WithMultiplication(int x, bool parentheses = false) {
    if (parentheses && Equation.Contains(' '))
      Equation = $"({Equation}) * {x}";
    else
      Equation += $" * {x}";
    return this;
  }

  public EquationBuilder WithDivision(int x, bool parentheses = false) {
    if (parentheses && Equation.Contains(' '))
      Equation = $"({Equation}) / {x}";
    else
      Equation += $" / {x}";
    return this;
  }

  public EquationBuilder WithModulus(int x, bool parentheses = false) {
    if (parentheses && Equation.Contains(' '))
      Equation = $"({Equation}) % {x}";
    else
      Equation += $" % {x}";
    return this;
  }

  public EquationBuilder WithExponent(int x, bool parentheses = false) {
    if (parentheses && Equation.Contains(' '))
      Equation = $"({Equation}) ^ {x}";
    else
      Equation += $" ^ {x}";
    return this;
  }

  public double Evaluate() {
    var expr = new Expression(Equation);
    return expr.Evaluate() is double ? (double)(expr.Evaluate() ?? 0) : 0;
  }

  /// <summary>
  /// Estimated difficulty of the equation,
  /// with 1 being the hardest and 0 being the easiest.
  /// </summary>
  /// <returns></returns>
  public double Difficulty() {
    var length    = Equation.Length;
    var operators = Equation.Count(c => c is '+' or '-' or '*' or '/' or '%');
    var answer    = Evaluate();
    return Math.Min(1, (length * 0.2 + operators * 0.5 + answer * 0.3) / 10);
  }
}