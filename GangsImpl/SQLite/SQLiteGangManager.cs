using System.Data.Common;
using CounterStrikeSharp.API.Core;
using Dapper;
using GangsAPI.Data.Gang;
using GangsAPI.Services;
using GenericDB;
using Microsoft.Data.Sqlite;

namespace SQLite;

public class SQLiteGangManager(IPlayerManager playerMgr,
  string connectionString, string table = "gang_gangs", bool testing = false)
  : AbstractDBGangManager(playerMgr, connectionString, table, testing) {
  private readonly bool myTesting = testing;
  private readonly string myTable = table;
  private readonly string myConnectionString = connectionString;

  public override void Start(BasePlugin? plugin, bool hotReload) {
    Connection = CreateDbConnection(myConnectionString);

    try {
      Connection.Open();

      if (myTesting) Transaction = Connection.BeginTransaction();

      var command = Connection.CreateCommand();

      command.Transaction = Transaction;
      command.CommandText = myTesting ?
        $"CREATE TEMPORARY TABLE IF NOT EXISTS {myTable} (GangId INT PRIMARY KEY, Name VARCHAR(255) NOT NULL)" :
        $"CREATE TABLE IF NOT EXISTS {myTable} (GangId INT PRIMARY KEY, Name VARCHAR(255) NOT NULL)";

      command.ExecuteNonQuery();
    } catch (Exception e) {
      Transaction?.Rollback();
      throw new InvalidOperationException("Failed initializing the database",
        e);
    }
  }

  public override async Task<IGang?> CreateGang(string name, ulong owner) {
    if (CachedGangs.Any(g => g.Name == name)) return null;
    var query = $"INSERT INTO {myTable} (Name) VALUES (@name)";
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
    var query  = $"UPDATE {myTable} SET Name = @Name WHERE GangId = @GangId";
    await Connection.ExecuteAsync(query, new { gang.Name, gang.GangId },
      Transaction);
    return result;
  }

  override protected DbConnection CreateDbConnection(string connectionString) {
    return new SqliteConnection(connectionString);
  }
}