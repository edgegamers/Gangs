using CounterStrikeSharp.API.Modules.Commands;

namespace GangsAPI.Data.Command;

/// <summary>
///   A wrapper for command logic to simplify command handling
///   The wrapper is responsible for keeping track of who
///   executed a command, what the parameters were when executing,
///   and the execution context.
/// </summary>
public class CommandInfoWrapper {
  /// <summary>
  ///   The arguments the command consists of
  /// </summary>
  public readonly string[] Args;

  /// <summary>
  ///   The player that executed the command
  /// </summary>
  public readonly PlayerWrapper? CallingPlayer;

  /// <summary>
  ///   The calling context of the command
  /// </summary>
  public CommandCallingContext CallingContext;

  public CommandInfoWrapper(PlayerWrapper? executor, int offset = 0,
    params string[] args) {
    CallingPlayer = executor;
    Args          = args.Skip(offset).ToArray();
    if (offset == 0 && Args.Length > 0) Args[0] = args[0].ToLower();
  }

  public CommandInfoWrapper(CommandInfo info, int offset = 0) {
    CallingPlayer = info.CallingPlayer == null ?
      null :
      new PlayerWrapper(info.CallingPlayer);
    CallingContext = info.CallingContext;
    Args           = new string[info.ArgCount - offset];
    for (var i = 0; i < info.ArgCount - offset; i++)
      Args[i] = info.GetArg(i + offset);
    if (offset == 0 && Args.Length > 0) Args[0] = Args[0].ToLower();
  }

  public int ArgCount => Args.Length;
  public string this[int index] => Args[index];

  public string GetCommandString => string.Join(' ', Args);

  /// <summary>
  ///   Replies to the player who issued the command to
  ///   the proper channels depending on context
  /// </summary>
  /// <param name="message"></param>
  public void ReplySync(string message) {
    if (CallingPlayer == null) {
      Console.WriteLine(message);
      return;
    }

    if (!CallingPlayer.IsValid().GetAwaiter().GetResult()) {
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