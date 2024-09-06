using System.Data.Common;
using CounterStrikeSharp.API.Core;
using Dapper;
using GangsAPI.Data.Gang;
using GangsAPI.Services;
using GenericDB;
using Microsoft.Data.Sqlite;

namespace SQLite;

public class SQLiteGangManager(IPlayerManager playerMgr, string connectionString,
  string table = "gang_gangs", bool testing = false)
  : AbstractDBGangManager(playerMgr, connectionString, table, testing) {
  public override void Start(BasePlugin? plugin, bool hotReload) {
    Connection = CreateDbConnection(connectionString);

    Connection.Open();

    if (testing) Transaction = Connection.BeginTransaction();

    try {
      var command = Connection.CreateCommand();

      command.Transaction = Transaction;
      command.CommandText = testing ?
        $"CREATE TEMPORARY TABLE IF NOT EXISTS {table} (GangId INT PRIMARY KEY, Name VARCHAR(255) NOT NULL)" :
        $"CREATE TABLE IF NOT EXISTS {table} (GangId INT PRIMARY KEY, Name VARCHAR(255) NOT NULL)";

      command.ExecuteNonQuery();
    } catch (Exception e) {
      Transaction?.Rollback();
      throw new InvalidOperationException("Failed initializing the database",
        e);
    }
  }

  public override async Task<IGang?> CreateGang(string name, ulong owner) {
    if (CachedGangs.Any(g => g.Name == name)) return null;
    var query = $"INSERT INTO {table} (Name) VALUES (@name)";
    var result =
      await Connection.ExecuteAsync(query, new { name }, Transaction);
    if (result == 0) return null;
    var id =
      await Connection.ExecuteScalarAsync<int>("SELECT last_insert_rowid()",
        transaction: Transaction);
    var gang = new DBGang(id, name);
    CachedGangs.Add(gang);
    return gang.Clone() as IGang;
  }

  public override async Task<bool> UpdateGang(IGang gang) {
    var result = await base.UpdateGang(gang);
    var query =
      $"UPDATE {table} SET Name = @Name WHERE GangId = @GangId";
    await Connection.ExecuteAsync(query, new { gang.Name, gang.GangId },
      transaction: Transaction);
    return result;
  }

  override protected DbConnection CreateDbConnection(string connectionString) {
    return new SqliteConnection(connectionString);
  }
}