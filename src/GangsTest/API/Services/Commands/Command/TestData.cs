﻿using System.Collections;
using Commands;
using Commands.Gang;
using GangsAPI;
using GangsAPI.Services.Gang;
using GangsAPI.Services.Player;
using GangsTest.TestLocale;
using Mock;

namespace GangsTest.API.Services.Commands.Command;

public class TestData : IEnumerable<object[]> {
  private static readonly IPlayerManager playerMgr = new MockPlayerManager();
  private static readonly IGangManager manager = new MockGangManager(playerMgr);

  private static readonly IPlayerStatManager statMgr =
    new MockInstanceStatManager();

  private readonly IBehavior[] behaviors = [
    new CreateCommand(manager), new HelpCommand(),
    new GangCommand(manager, StringLocalizer.Instance),
    new BalanceCommand(statMgr, StringLocalizer.Instance)
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}