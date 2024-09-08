using GangsAPI.Data;

namespace GangsImpl;

public class EnvDBConfig : IDBConfig {
  public string ConnectionString
    => Environment.GetEnvironmentVariable("DB_GANGS_CONNECTION")
      ?? "Host=localhost;User=root;Database=gangs";

  public string TablePrefix
    => Environment.GetEnvironmentVariable("DB_GANGS_PREFIX") ?? "cs2_gangs";

  public bool Testing => false;
}