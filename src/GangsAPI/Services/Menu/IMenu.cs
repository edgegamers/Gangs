using GangsAPI.Data;

namespace GangsAPI.Services.Menu;

public interface IMenu {
  Task Open(PlayerWrapper player);
  Task Close(PlayerWrapper player);
  Task OnInput(PlayerWrapper player, int input);
}