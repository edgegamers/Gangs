using GangsAPI.Data;

namespace Menu;

public class NativeSenders {
  public static Func<PlayerWrapper, string, Task> Chat
    => (player, message) => {
      player.PrintToChat(message);
      return Task.CompletedTask;
    };

  public static Func<PlayerWrapper, string, Task> Console
    => (player, message) => {
      player.PrintToConsole(message);
      return Task.CompletedTask;
    };

  public static Func<PlayerWrapper, string, Task> Center
    => (player, message) => {
      player.PrintToCenter(message);
      return Task.CompletedTask;
    };
}