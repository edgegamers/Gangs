using NCalc;

namespace EcoRewards.EcoMath;

public class EquationBuilder {
  public EquationBuilder(int start) { Equation = start.ToString(); }
  public string Equation { get; private set; }

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
    if (x == 0) x = 1;
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
      Equation = $"({Equation}) ** {x}";
    else
      Equation += $" ** {x}";
    return this;
  }

  public double Evaluate() {
    var expr = new Expression(Equation.Replace(" ", ""));
    return Convert.ToDouble(expr.Evaluate());
  }

  /// <summary>
  ///   Estimated difficulty of the equation,
  ///   with 1 being the hardest and 0 being the easiest.
  /// </summary>
  /// <returns></returns>
  public double Difficulty() {
    var length           = Equation.Length;
    var simpleOperators  = Equation.Count(c => c is '+' or '-');
    var complexOperators = Equation.Count(c => c is '*' or '/' or '%');
    var answer           = Evaluate();
    var diff = (length * 0.2 + simpleOperators * 0.5 + complexOperators * 0.7)
      / 10;
    if (answer % 1 != 0) diff += 0.2;

    return Math.Min(1, diff);
  }
}