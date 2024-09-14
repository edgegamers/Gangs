namespace GangsAPI.Exceptions;

public class GangException : Exception {
  public GangException() : base("") { }
  public GangException(string message) : base(message) { }
}