﻿using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Services.Commands;
using GangsAPI.Services.Gang;

namespace Commands.Gang;

// create [name]
public class CreateCommand(IGangManager gangs) : ICommand {
  public string Name => "create";
  public string Description => "Creates a new gang";
  public string Usage => "[name]";

  public async Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    if (executor == null) return CommandResult.PLAYER_ONLY;

    if (info.ArgCount < 2) {
      info.ReplySync("Please provide a name for the gang");
      return CommandResult.INVALID_ARGS;
    }

    var name = string.Join(' ', info.ArgString.Split(" "));

    if (await gangs.GetGang(executor.Steam) != null) {
      info.ReplySync("You are already in a gang");
      return CommandResult.ERROR;
    }

    if ((await gangs.GetGangs()).Any(g => g.Name == name)) {
      info.ReplySync($"Gang '{name}' already exists");
      return CommandResult.ERROR;
    }

    var newGang = await gangs.CreateGang(name, executor.Steam);

    if (newGang == null) {
      info.ReplySync("Failed to create gang");
      return CommandResult.ERROR;
    }

    info.ReplySync($"Gang '{name}' (#{newGang.GangId}) created successfully");
    return CommandResult.SUCCESS;
  }
}