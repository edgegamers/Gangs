using CounterStrikeSharp.API.Modules.Commands;

namespace GangsAPI.Data.Command;

public class CommandInfoWrapper {
  public readonly string[] Args;

  public readonly CommandCallingContext CallingContext =
    CommandCallingContext.Console;

  public readonly PlayerWrapper? CallingPlayer;
  private readonly int offset;

  public CommandInfoWrapper(PlayerWrapper? executor, int offset = 0,
    params string[] args) {
    CallingPlayer = executor;
    this.offset   = offset;
    Args          = args.Skip(offset - 1).ToArray();
    if (offset == 0 && Args.Length > 0) Args[0] = args[0].ToLower();
  }

  public CommandInfoWrapper(CommandInfo info, int offset = 0) {
    CallingPlayer = info.CallingPlayer == null ?
      null :
      new PlayerWrapper(info.CallingPlayer);
    this.offset    = offset;
    CallingContext = info.CallingContext;
    Args           = new string[info.ArgCount];
    for (var i = offset; i < info.ArgCount; i++)
      Args[i - offset] = info.GetArg(i);
  }

  public int ArgCount => Args.Length - offset;
  public string this[int index] => Args[index + offset];

  public string ArgString
    => string.Join(' ', GetCommandString.Split(' ').Skip(offset + 1));

  public string GetCommandString => string.Join(' ', Args.Skip(offset));

  public void ReplySync(string message) {
    if (CallingPlayer == null) {
      Console.WriteLine(message);
      return;
    }

    if (!CallingPlayer.IsValid) {
      Console.Error.WriteLine(
        $"Player {CallingPlayer} is not valid, cannot reply to command");
      return;
    }

    if (CallingContext == CommandCallingContext.Console)
      CallingPlayer.PrintToConsole(message);
    else
      CallingPlayer.PrintToChat(message);
  }
}