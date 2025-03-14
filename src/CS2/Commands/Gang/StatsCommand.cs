﻿using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Exceptions;
using GangsAPI.Services.Commands;

namespace Commands.Gang;

public class StatsCommand : ICommand {
  public string Name => "stats";
  public string? Description => "View the stats of your gang";

  public Task<CommandResult> Execute(PlayerWrapper? executor,
    CommandInfoWrapper info) {
    throw new GangException("Not implemented");
  }
}