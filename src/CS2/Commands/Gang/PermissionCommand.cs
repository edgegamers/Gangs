using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;
using GangsAPI.Services.Commands;

namespace Commands.Gang;

public class PermissionCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "permission";

  public override string[] Aliases => ["permission", "perm", "perms"];

  public override string[] Usage
    => ["grant <rank> <perm>", "revoke <rank> <perm>", "set <rank> <int>"];

  override protected Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    throw new GangException("Not implemented");
  }
}