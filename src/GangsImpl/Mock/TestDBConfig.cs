using GangsAPI.Data;

namespace Mock;

public class TestDBConfig : IDBConfig {
  public string ConnectionString { get; } =
    Environment.GetEnvironmentVariable("DB_GANGS_CONNECTION")
    ?? "Host=localhost;User=root;Database=gangs";

  public string TablePrefix => "gangs_unit_test";

  public bool Testing => true;
}