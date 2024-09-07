using Mock;

namespace GenericDB;

/// <summary>
///   Dapper-compatible representation of a gang.
/// </summary>
public class DBGang(int id, string name) : MockGang(id, name) {
  // public DBGang(IGang gang) : this(gang.GangId, gang.Name) { }
}