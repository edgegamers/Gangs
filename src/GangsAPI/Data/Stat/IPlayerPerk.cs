namespace GangsAPI.Data.Stat;

public interface IPlayerPerk : IPerk {
  void ApplyTo(ulong steam);
  void RevokedFrom(ulong steam);
}