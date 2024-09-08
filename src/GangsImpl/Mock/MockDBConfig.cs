using GangsAPI.Data;

namespace Mock;

public class MockDBConfig(string connectionString, string tablePrefix,
  bool testing) : IDBConfig {
  public string ConnectionString { get; } = connectionString;
  public string TablePrefix { get; } = tablePrefix;
  public bool Testing { get; } = testing;
}