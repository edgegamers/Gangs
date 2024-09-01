namespace GangsAPI.Permissions;

/// <summary>
///   The dictator of permissions.
/// </summary>
public interface IPermDictator : IPluginBehavior {
  bool CanTarget(IGangRank source, IGangRank target);
  IGangRank CreateRank(string name, int rank, IGangRank.Permissions perms = 0);
}