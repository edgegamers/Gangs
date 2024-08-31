using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace GangsAPI.Data;

public class CommandInfoWrapper(CCSPlayerController? executor,
  params string[] args) {
  public readonly CCSPlayerController? CallingPlayer = executor;

  public readonly CommandCallingContext CallingContext =
    CommandCallingContext.Console;

  public CommandInfoWrapper(CommandInfo info) : this(info.CallingPlayer,
    info.ArgString.Split(" ")) {
    CallingContext = info.CallingContext;
  }

  public int ArgCount => args.Length;
  public string this[int index] => args[index];
  public string ArgString => string.Join(' ', args);

  public void ReplyToCommand(string message) {
    if (CallingPlayer == null) {
      Console.WriteLine(message);
      return;
    }

    if (CallingContext == CommandCallingContext.Console)
      CallingPlayer.PrintToConsole(message);
    else
      CallingPlayer.PrintToChat(message);
  }
}