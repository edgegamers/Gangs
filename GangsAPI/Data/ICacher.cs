namespace GangsAPI.Data;

public interface ICacher {
  void ClearCache();
  Task Load();
}