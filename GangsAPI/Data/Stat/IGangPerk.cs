namespace GangsAPI.Data.Stat;

public interface IGangPerk : IPerk {
  void ApplyTo(int id);
  void RevokedFrom(int id);
}