namespace GangsAPI.Exceptions;

public class GangException(string message) : Exception(message) {
  public GangException() : this("") { }
}