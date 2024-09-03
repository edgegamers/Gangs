using System.Data.Common;
using CounterStrikeSharp.API.Core;
using Dapper;
using GangsAPI.Data.Gang;
using Mock;
using Xunit;

namespace GenericDB;

public abstract class AbstractDBGangManager(string connectionString,
  string table = "gang_gangs", bool testing = false) : MockGangManager {
  private DbConnection connection = null!;
  private DbTransaction? transaction;
  public override void ClearCache() { cachedGangs.Clear(); }

  public override void Start(BasePlugin? plugin, bool hotReload) {
    Assert.NotNull(table);
    connection = CreateDbConnection(connectionString);

    connection.Open();

    if (testing) transaction = connection.BeginTransaction();

    try {
      var command = connection.CreateCommand();

      command.Transaction = transaction;
      command.CommandText = testing ?
        $"CREATE TEMPORARY TABLE IF NOT EXISTS {table} (GangId INT NOT NULL AUTO_INCREMENT PRIMARY KEY, Name VARCHAR(255) NOT NULL)" :
        $"CREATE TABLE IF NOT EXISTS {table} (GangId INT NOT NULL PRIMARY KEY, Name VARCHAR(255) NOT NULL)";

      command.ExecuteNonQuery();
    } catch (Exception e) {
      transaction?.Rollback();
      throw new InvalidOperationException("Failed initializing the database",
        e);
    }
  }

  abstract protected DbConnection CreateDbConnection(string connectionString);

  public override async Task Load() {
    var query = $"SELECT * FROM {table}";
    var gangs = await connection.QueryAsync<DBGang>(query);
    foreach (var gang in gangs) cachedGangs.Add(gang);
  }

  public override async Task<bool> UpdateGang(IGang gang) {
    await base.UpdateGang(gang);
    var query = $"UPDATE {table} SET Name = @Name WHERE GangId = @GangId";
    return await connection.ExecuteAsync(query, new { gang.Name, gang.GangId },
      transaction) > 0;
  }

  public override async Task<bool> DeleteGang(int id) {
    await base.DeleteGang(id);
    var query = $"DELETE FROM {table} WHERE GangId = @id";
    return await connection.ExecuteAsync(query, new { id }, transaction) > 0;
  }

  public override async Task<IGang?> CreateGang(string name, ulong owner) {
    Assert.NotNull(table);
    if (cachedGangs.Any(g => g.Name == name)) return null;
    var query = $"INSERT INTO {table} (Name) VALUES (@name)";
    var result =
      await connection.ExecuteAsync(query, new { name }, transaction);
    if (result == 0) return null;
    var id = await connection.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID()",
      transaction: transaction);
    var gang = new DBGang(id, name, owner);
    cachedGangs.Add(gang);
    return gang.Clone() as IGang;
  }

  public override void Dispose() {
    transaction?.Dispose();
    connection.Dispose();
  }
}