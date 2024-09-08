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
    this.Args     = args;
    if (offset == 0 && args.Length > 0) this.Args[0] = args[0].ToLower();
  }

  public CommandInfoWrapper(CommandInfo info, int offset = 0) {
    CallingPlayer = info.CallingPlayer == null ?
      null :
      new PlayerWrapper(info.CallingPlayer);
    this.offset    = offset;
    CallingContext = info.CallingContext;
    Args           = new string[info.ArgCount];
    for (var i = 0; i < info.ArgCount; i++) Args[i] = info.GetArg(i);
    if (offset == 0 && info.ArgCount > 0) Args[0]   = info.GetArg(0).ToLower();
  }

  public CommandInfoWrapper(CommandInfoWrapper info, int offset) : this(
    info.CallingPlayer, offset, info.Args) {
    CallingContext = info.CallingContext;
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