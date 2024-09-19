namespace GangsAPI.Exceptions;

public class PlayerNotFoundException(ulong? steam)
  : GangException($"Failed to fetch/create Gang Player for {steam}") { }