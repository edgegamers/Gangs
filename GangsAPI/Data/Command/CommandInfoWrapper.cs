using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace GangsAPI.Data.Command;

public class CommandInfoWrapper(PlayerWrapper? executor, int offset = 0,
  params string[] args) {
  public readonly PlayerWrapper? CallingPlayer = executor;

  public readonly CommandCallingContext CallingContext =
    CommandCallingContext.Console;

  public CommandInfoWrapper(CommandInfo info, int offset = 0) : this(
    info.CallingPlayer == null ? null : new PlayerWrapper(info.CallingPlayer),
    offset, info.ArgString.Split(" ")) {
    CallingContext = info.CallingContext;
  }

  public CommandInfoWrapper(CommandInfoWrapper info, int offset) : this(
    info.CallingPlayer, offset, info.ArgString.Split(" ")) {
    CallingContext = info.CallingContext;
  }

  public int ArgCount => args.Length - offset;
  public string this[int index] => args[index + offset];

  public string ArgString
    => string.Join(' ', GetCommandString.Split(' ').Skip(offset));

  public string GetCommandString => string.Join(' ', args.Skip(offset));

  public void ReplyToCommandSync(string message) {
    if (CallingPlayer == null) {
      Console.WriteLine(message);
      return;
    }

    new Task(() => {
      if (!CallingPlayer.IsValid) {
        Console.Error.WriteLine(
          $"Player {CallingPlayer} is not valid, cannot reply to command");
        return;
      }

      if (CallingContext == CommandCallingContext.Console)
        CallingPlayer.PrintToConsole(message);
      else
        CallingPlayer.PrintToChat(message);
    }).RunSynchronously();
  }
}