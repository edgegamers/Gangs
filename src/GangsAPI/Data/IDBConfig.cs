namespace GangsAPI.Data;

public interface IDBConfig {
  string ConnectionString { get; }
  string TablePrefix { get; }
  bool Testing { get; } // TODO: Use environment variables
}