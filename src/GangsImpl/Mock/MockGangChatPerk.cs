using CounterStrikeSharp.API.Modules.Admin;
using GangsAPI;
using GangsAPI.Data.Gang;
using GangsAPI.Data.Stat;
using GangsAPI.Perks;
using GangsAPI.Services.Menu;
using GangsAPI.Services.Player;
using Microsoft.Extensions.DependencyInjection;

namespace Mock;

public class MockGangChatPerk(IServiceProvider provider)
  : IGangChatPerk, IPluginBehavior {
  public bool Equals(IStat? other) { throw new NotImplementedException(); }
  public string StatId => "mock_gang_chat";
  public string Name => "Mock Gang Chat";
  public string? Description => "Mock chat with your gang members";
  public Type ValueType => typeof(bool);

  private readonly Dictionary<int, List<string>> history = new();

  private readonly IPlayerManager players =
    provider.GetRequiredService<IPlayerManager>();

  public Task<int?> GetCost(IGangPlayer player) {
    return Task.FromResult<int?>(10000);
  }

  public Task OnPurchase(IGangPlayer player) { return Task.CompletedTask; }

  public Task<IMenu?> GetMenu(IGangPlayer player) {
    return Task.FromResult<IMenu?>(null);
  }

  public async Task SendGangChat(string name, IGang gang, string message) {
    if (!history.TryGetValue(gang.GangId, out var value)) {
      value                = [];
      history[gang.GangId] = value;
    }

    value.Add($"{name}: {message}");
  }

  public void ClearChatHistory(IGang gang) { history.Remove(gang.GangId); }

  public IEnumerable<string> GetChatHistory(IGang gang) {
    return history.TryGetValue(gang.GangId, out var value) ? value : [];
  }
}