﻿using System.Collections;
using GangsAPI;
using GangsAPI.Services.Player;
using GangsTest.TestLocale;
using Mock;

namespace GangsTest.API.Services.Commands.CommandManager;

public class TestData : IEnumerable<object[]> {
  private static readonly IPlayerManager playerMgr = new MockPlayerManager();

  private readonly IBehavior[] behaviors = [
    new MockCommandManager(StringLocalizer.Instance),
    new global::Commands.CommandManager(new MockGangManager(playerMgr),
      StringLocalizer.Instance)
  ];

  public TestData() {
    foreach (var behavior in behaviors) behavior.Start();
  }

  public IEnumerator<object[]> GetEnumerator() {
    return behaviors.Select(behavior => (object[]) [behavior]).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
}