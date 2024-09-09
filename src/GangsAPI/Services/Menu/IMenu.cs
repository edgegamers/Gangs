using GangsAPI.Data;

namespace GangsAPI.Services.Menu;

public interface IMenu {
  Task Open(PlayerWrapper player);
  Task Close(PlayerWrapper player);
  Task AcceptInput(PlayerWrapper player, int input);
}