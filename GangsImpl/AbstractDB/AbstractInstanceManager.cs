using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using CounterStrikeSharp.API.Core;
using Dapper;

namespace GenericDB;

public abstract class AbstractInstanceManager<TK>(string connectionString,
  string table_prefix, bool testing = false) {
  protected DbConnection Connection;
  abstract protected string PrimaryKey { get; }
  private string primaryTypeString => GetDBType(typeof(TK));

  public void ClearCache() { }

  public Task Load() { return Task.CompletedTask; }

  public async Task<(bool, TV?)> Get<TV>(TK key, string statId) {
    await createTable<TV>(statId);
    try {
      var result = await Connection.QuerySingleAsync<TV>(
        $"SELECT {(typeof(TV).IsPrimitive ? statId : getFieldNames<TV>())} FROM {table_prefix}_{statId} WHERE {PrimaryKey} = @{PrimaryKey}",
        new { GangId = key });
      return (true, result);
    } catch (InvalidOperationException) { return (false, default); }
  }

  public async Task<bool> Set<TV>(TK key, string statId, TV value) {
    await createTable<TV>(statId);
    var fields = typeof(TV)
     .GetProperties(BindingFlags.Public | BindingFlags.Instance)
     .ToList();

    var columns = getFieldNames<TV>();
    var values  = getFieldNames<TV>("@");

    if (typeof(TV).IsPrimitive) {
      columns = statId;
      values  = $"@{statId}";
    }

    var onDuplicate = string.Join(", ",
      fields.Select(f => $"{f.Name} = @{f.Name}"));
    if (typeof(TV).IsPrimitive) onDuplicate = $"{statId} = @{statId}";
    var cmd =
      $"INSERT INTO {table_prefix}_{statId} ({PrimaryKey}, {columns}) VALUES (@{PrimaryKey}, {values}) ON DUPLICATE KEY UPDATE {onDuplicate}";

    Debug.WriteLine(cmd);

    var fieldValues = new DynamicParameters();
    fieldValues.Add("@" + PrimaryKey, key);

    if (typeof(TV).IsPrimitive)
      fieldValues.Add($"@{statId}", value);
    else
      foreach (var field in fields)
        fieldValues.Add($"@{field.Name}", field.GetValue(value));

    Console.WriteLine(cmd, fieldValues);
    await Connection.ExecuteAsync(cmd, fieldValues);
    return true;
  }

  public async Task<bool> Remove(TK key, string statId) {
    try {
      var dynamicParameters = new DynamicParameters();
      dynamicParameters.Add(PrimaryKey, key);
      await Connection.ExecuteAsync(
        $"DELETE FROM {table_prefix}_{statId} WHERE {PrimaryKey} = @{PrimaryKey}",
        dynamicParameters);
      return true;
    } catch (DbException) { return false; }
  }

  public void Start(BasePlugin? plugin, bool hotReload) {
    Connection = CreateDbConnection(connectionString);

    Connection.Open();
  }

  virtual protected string GetDBType(Type type) {
    return type.Name switch {
      "Int32" => "INT",
      "String" => "VARCHAR(255)",
      "Single" => "FLOAT",
      "Boolean" => "BOOLEAN",
      "Int64" => "BIGINT",
      "UInt32" => "INT UNSIGNED",
      "UInt64" => "BIGINT UNSIGNED",
      "Double" => "DOUBLE",
      "Decimal" => "DECIMAL",
      "DateTime" => "DATETIME",
      "TimeSpan" => "TIME",
      "Byte" => "TINYINT",
      "SByte" => "TINYINT",
      "UInt16" => "SMALLINT UNSIGNED",
      "Int16" => "SMALLINT",
      "Guid" => "CHAR(36)",
      "Char" => "CHAR",
      _ => throw new NotImplementedException($"Unknown type {type.Name}")
    };
  }

  private async Task createTable<TV>(string id) {
    var fields = typeof(TV)
     .GetProperties(BindingFlags.Public | BindingFlags.Instance)
     .ToList();

    var cmd = testing ?
      $"CREATE TEMPORARY TABLE IF NOT EXISTS {table_prefix}_{id} ({PrimaryKey} {primaryTypeString} NOT NULL PRIMARY KEY, " :
      $"CREATE TABLE IF NOT EXISTS {table_prefix}_{id} ({PrimaryKey} {primaryTypeString} NOT NULL PRIMARY KEY, ";

    if (typeof(TV).IsPrimitive) {
      cmd += $"{id} {GetDBType(typeof(TV))}";
      if (Nullable.GetUnderlyingType(typeof(TV)) == null) cmd += " NOT NULL)";
    } else {
      foreach (var field in fields) {
        cmd += $"{field.Name} {GetDBType(field.PropertyType)}";
        if (Nullable.GetUnderlyingType(field.PropertyType) == null)
          cmd += " NOT NULL";
        cmd += ", ";
      }

      cmd = cmd[..^2] + ")";
    }

    Console.WriteLine(cmd);

    await Connection.ExecuteAsync(cmd);
  }

  private string getFieldNames<TV>(string prefix = "") {
    var fields = typeof(TV)
     .GetProperties(BindingFlags.Public | BindingFlags.Instance)
     .ToList();

    return string.Join(", ", fields.Select(f => $"{prefix}{f.Name}"));
  }

  abstract protected DbConnection CreateDbConnection(string connectionString);
}