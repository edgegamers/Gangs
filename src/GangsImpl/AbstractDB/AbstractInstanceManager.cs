using System.Data.Common;
using System.Reflection;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Dapper;
using GangsAPI.Extensions;

namespace GenericDB;

public abstract class AbstractInstanceManager<TK>(string connectionString,
  string table_prefix, bool testing = false) where TK : notnull {
  protected DbConnection Connection = null!;
  abstract protected string PrimaryKey { get; }
  private string primaryTypeString => GetDBType(typeof(TK));
  private bool busy;

  private readonly Dictionary<string, Dictionary<TK, object>> cache = new();

  public async Task<(bool, TV?)> Get<TV>(TK key, string statId) {
    if (cache.TryGetValue(statId, out var dict)
      && dict.TryGetValue(key, out var value))
      return (true, (TV)value);

    if (busy) {
      await Server.NextFrameAsync(() => {
        Server.PrintToConsole($"ERROR: Get called while busy ({key}:{statId})");
        Task.Run(async () => {
          await Task.Delay(50);
          return Get<TV>(key, statId);
        });
      });
    }

    await createTable<TV>(statId);
    try {
      var dynamic = new DynamicParameters();
      dynamic.Add(PrimaryKey, key);
      var result = await Connection.QuerySingleAsync<TV>(
        $"SELECT {(typeof(TV).IsBasicallyPrimitive() ? statId : GetFieldNames<TV>())} FROM {table_prefix}_{statId} WHERE {PrimaryKey} = @{PrimaryKey}",
        dynamic);
      if (result == null) return (true, result);
      if (!cache.ContainsKey(statId))
        cache[statId] = new Dictionary<TK, object>();
      cache[statId][key] = result;
      return (true, result);
    } catch (InvalidOperationException e) {
      if (!e.Message.Contains("Sequence contains no elements")) throw;
      return (false, default);
    }
  }

  virtual protected string GenerateInsertQuery<TV>(string statId,
    IList<PropertyInfo> properties) {
    var columns = GetFieldNames<TV>();
    var values  = GetFieldNames<TV>("@");

    if (typeof(TV).IsBasicallyPrimitive()) {
      columns = statId;
      values  = $"@{statId}";
    }

    var onDuplicate = string.Join(", ",
      properties.Select(f => $"{f.Name} = @{f.Name}"));
    if (typeof(TV).IsBasicallyPrimitive())
      onDuplicate = $"{statId} = @{statId}";
    return
      $"INSERT INTO {table_prefix}_{statId} ({PrimaryKey}, {columns}) VALUES (@{PrimaryKey}, {values}) ON DUPLICATE KEY UPDATE {onDuplicate}";
  }


  public async Task<bool> Set<TV>(TK key, string statId, TV value) {
    if (busy) {
      await Server.NextFrameAsync(() => {
        Server.PrintToConsole(
          $"ERROR: Set called while busy ({key}:{statId} = {value})");
        Task.Run(async () => {
          await Task.Delay(50);
          return Set(key, statId, value);
        });
      });
    }

    await createTable<TV>(statId);

    var fields = typeof(TV)
     .GetProperties(BindingFlags.Public | BindingFlags.Instance)
     .ToList();

    var cmd = GenerateInsertQuery<TV>(statId, fields);

    var fieldValues = new DynamicParameters();
    fieldValues.Add("@" + PrimaryKey, key);

    if (typeof(TV).IsBasicallyPrimitive())
      fieldValues.Add($"@{statId}", value);
    else
      foreach (var field in fields)
        fieldValues.Add($"@{field.Name}", field.GetValue(value));

    if (value != null) {
      if (!cache.ContainsKey(statId))
        cache[statId] = new Dictionary<TK, object>();
      cache[statId][key] = value;
    }

    await Connection.ExecuteAsync(cmd, fieldValues);
    busy = false;
    return true;
  }

  public async Task<bool> Remove(TK key, string statId) {
    try {
      var dynamicParameters = new DynamicParameters();
      dynamicParameters.Add(PrimaryKey, key);
      await Connection.ExecuteAsync(
        $"DELETE FROM {table_prefix}_{statId} WHERE {PrimaryKey} = @{PrimaryKey}",
        dynamicParameters);
      if (!cache.TryGetValue(statId, out var value)) return true;
      value.Remove(key);
      return true;
    } catch (DbException e) {
      if (e.Message.Contains("no such table")) return false;
      if (e.Message.EndsWith("doesn't exist")) return false;
      throw;
    }
  }

  public void Start(BasePlugin? plugin, bool hotReload) {
    Connection = CreateDbConnection(connectionString);

    Connection.Open();
  }

  virtual protected string GetDBType(Type type) {
    if (type.IsEnum) return "INT";
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
    var cmd = testing ?
      $"CREATE TEMPORARY TABLE IF NOT EXISTS {table_prefix}_{id} ({PrimaryKey} {primaryTypeString} NOT NULL PRIMARY KEY, " :
      $"CREATE TABLE IF NOT EXISTS {table_prefix}_{id} ({PrimaryKey} {primaryTypeString} NOT NULL PRIMARY KEY, ";

    if (typeof(TV).IsBasicallyPrimitive()) {
      cmd += $"{id} {GetDBType(typeof(TV))}";
      if (Nullable.GetUnderlyingType(typeof(TV)) == null) cmd += " NOT NULL)";
    } else {
      var fields = typeof(TV)
       .GetProperties(BindingFlags.Public | BindingFlags.Instance)
       .ToList();
      foreach (var field in fields) {
        cmd += $"{field.Name} {GetDBType(field.PropertyType)}";
        if (Nullable.GetUnderlyingType(field.PropertyType) == null)
          cmd += " NOT NULL";
        cmd += ", ";
      }

      cmd = cmd[..^2] + ")";
    }

    await Connection.ExecuteAsync(cmd);
  }

  protected string GetFieldNames<TV>(string prefix = "") {
    var fields = typeof(TV)
     .GetProperties(BindingFlags.Public | BindingFlags.Instance)
     .ToList();

    return string.Join(", ", fields.Select(f => $"{prefix}{f.Name}"));
  }

  abstract protected DbConnection CreateDbConnection(string connectionString);
}