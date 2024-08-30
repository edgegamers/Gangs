namespace GangsAPI.Permissions;

/// <summary>
/// The dictator of permissions.
/// </summary>
public interface IPermDictator {
  bool CanTarget(IGangRank source, IGangRank target);
}