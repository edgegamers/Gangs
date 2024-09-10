using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Commands;

namespace GangsAPI.Data.Command;

public class CommandInfoWrapper {
  public readonly string[] Args;

  public readonly PlayerWrapper? CallingPlayer;
  private readonly int offset;
  private bool isInChat;

  public CommandInfoWrapper(PlayerWrapper? executor, int offset = 0,
    params string[] args) {
    CallingPlayer = executor;
    this.offset   = offset;
    Args          = args.Skip(offset).ToArray();
    if (offset == 0 && Args.Length > 0) Args[0] = args[0].ToLower();
  }

  public CommandInfoWrapper(CommandInfo info, int offset = 0) {
    CallingPlayer = info.CallingPlayer == null ?
      null :
      new PlayerWrapper(info.CallingPlayer);
    this.offset = offset;
    isInChat    = info.CallingContext == CommandCallingContext.Chat;
    Args        = new string[info.ArgCount - offset];
    for (var i = 0; i < info.ArgCount - offset; i++)
      Args[i] = info.GetArg(i + offset);
    if (offset == 0 && Args.Length > 0) Args[0] = Args[0].ToLower();
  }

  public int ArgCount => Args.Length;
  public string this[int index] => Args[index];

  public string GetCommandString => string.Join(' ', Args);

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

    if (isInChat)
      CallingPlayer.PrintToChat(message);
    else
      CallingPlayer.PrintToConsole(message);
  }
}