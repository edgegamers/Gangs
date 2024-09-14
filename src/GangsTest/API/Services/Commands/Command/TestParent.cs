﻿using GangsAPI.Data;
using GangsAPI.Extensions;
using GangsAPI.Services.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace GangsTest.API.Services.Commands.Command;

public abstract class TestParent {
  protected readonly ICommand Command;
  protected readonly ICommandManager Commands;

  protected readonly PlayerWrapper TestPlayer =
    new(new Random().NextULong(), "Test Player");

  protected TestParent(IServiceProvider provider, ICommand command) {
    Commands = provider.GetRequiredService<ICommandManager>();
    Command  = command;
    Commands.RegisterCommand(command);
    Commands.Start();
  }
}