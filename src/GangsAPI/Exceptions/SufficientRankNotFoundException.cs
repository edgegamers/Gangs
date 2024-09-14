using GangsAPI.Permissions;

namespace GangsAPI.Exceptions;

public class SufficientRankNotFoundException : GangException {
  public SufficientRankNotFoundException(int gang, Perm perm) : base(
    $"Gang #{gang} does not have a rank with permission {perm}") { }
}