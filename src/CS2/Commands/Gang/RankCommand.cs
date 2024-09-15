using GangsAPI.Data;
using GangsAPI.Data.Command;
using GangsAPI.Data.Gang;
using GangsAPI.Exceptions;

namespace Commands.Gang;

public class RankCommand(IServiceProvider provider)
  : GangedPlayerCommand(provider) {
  public override string Name => "rank";

  public override string[] Usage
    => ["", "create <name> <rank>", "delete <rank>"];

  override protected Task<CommandResult> Execute(PlayerWrapper executor,
    IGangPlayer player, CommandInfoWrapper info) {
    throw new GangException("Not implemented");
  }
}