﻿using CounterStrikeSharp.API.Modules.Commands;

namespace GangsAPI.Data.Command;

public class CommandInfoWrapper {
  private readonly string[] args;

  public readonly CommandCallingContext CallingContext =
    CommandCallingContext.Console;

  public readonly PlayerWrapper? CallingPlayer;
  private readonly int offset;

  public CommandInfoWrapper(PlayerWrapper? executor, int offset = 0,
    params string[] args) {
    CallingPlayer = executor;
    this.offset   = offset;
    this.args     = args;
    if (offset == 0 && args.Length > 0) this.args[0] = args[0].ToLower();
  }

  public CommandInfoWrapper(CommandInfo info, int offset = 0) {
    CallingPlayer = info.CallingPlayer == null ?
      null :
      new PlayerWrapper(info.CallingPlayer);
    this.offset    = offset;
    CallingContext = info.CallingContext;
    args           = new string[info.ArgCount];
    for (var i = 0; i < info.ArgCount; i++) args[i] = info.GetArg(i);
    if (offset == 0 && info.ArgCount > 0) args[0]   = info.GetArg(0).ToLower();
  }

  public CommandInfoWrapper(CommandInfoWrapper info, int offset) : this(
    info.CallingPlayer, offset, info.ArgString.Split(" ")) {
    CallingContext = info.CallingContext;
  }

  public int ArgCount => args.Length - offset;
  public string this[int index] => args[index + offset];

  public string ArgString
    => string.Join(' ', GetCommandString.Split(' ').Skip(offset + 1));

  public string GetCommandString => string.Join(' ', args.Skip(offset));

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