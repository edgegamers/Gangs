using System.Data.Common;
using CounterStrikeSharp.API.Core;
using Dapper;
using GangsAPI.Data.Gang;
using GangsAPI.Services;
using Mock;

namespace GenericDB;

public abstract class AbstractDBGangManager(IPlayerManager playerMgr,
  string connectionString, string table = "gang_gangs", bool testing = false)
  : MockGangManager(playerMgr) {
  protected DbConnection Connection = null!;
  protected DbTransaction? Transaction;
  public override void ClearCache() { CachedGangs.Clear(); }

  public override void Start(BasePlugin? plugin, bool hotReload) {
    Connection = CreateDbConnection(connectionString);

    Connection.Open();

    if (testing) Transaction = Connection.BeginTransaction();

    try {
      var command = Connection.CreateCommand();

      command.Transaction = Transaction;
      command.CommandText = testing ?
        $"CREATE TEMPORARY TABLE IF NOT EXISTS {table} (GangId INT NOT NULL AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255) NOT NULL)" :
        $"CREATE TABLE IF NOT EXISTS {table} (GangId INT NOT NULL AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255) NOT NULL)";

      command.ExecuteNonQuery();
    } catch (Exception e) {
      Transaction?.Rollback();
      throw new InvalidOperationException("Failed initializing the database",
        e);
    }
  }

  abstract protected DbConnection CreateDbConnection(string connectionString);

  public override async Task Load() {
    var query = $"SELECT * FROM {table}";
    var gangs = await Connection.QueryAsync<DBGang>(query);
    foreach (var gang in gangs) CachedGangs.Add(gang);
  }

  public override async Task<bool> UpdateGang(IGang gang) {
    var result = await base.UpdateGang(gang);
    var query  = $"UPDATE {table} SET Name = @Name WHERE GangId = @GangId";
    await Connection.ExecuteAsync(query, new { gang.Name, gang.GangId },
      Transaction);
    return result;
  }

  public override async Task<bool> DeleteGang(int id) {
    await base.DeleteGang(id);
    var query = $"DELETE FROM {table} WHERE GangId = @id";
    return await Connection.ExecuteAsync(query, new { id }, Transaction) > 0;
  }

  public override async Task<IGang?> CreateGang(string name, ulong owner) {
    if (CachedGangs.Any(g => g.Name == name)) return null;
    var query = $"INSERT INTO {table} (Name) VALUES (@name)";
    var result =
      await Connection.ExecuteAsync(query, new { name }, Transaction);
    if (result == 0) return null;
    var id = await Connection.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID()",
      transaction: Transaction);
    var gang = new DBGang(id, name);
    CachedGangs.Add(gang);
    return gang.Clone() as IGang;
  }

  public override void Dispose() {
    Transaction?.Dispose();
    Connection.Dispose();
  }
}