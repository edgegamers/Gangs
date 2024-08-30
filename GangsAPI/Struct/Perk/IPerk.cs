namespace GangsAPI.Struct.Perk;

public interface IPerk : IEqualityComparer<IPerk> {
  /// <summary>
  /// The unique identifier of the perk.
  /// </summary>
  string PerkId { get; }

  /// <summary>
  /// The name of the perk.
  /// </summary>
  string Name { get; }

  /// <summary>
  /// A description of the perk.
  /// </summary>
  string? Description { get; }

  bool IEqualityComparer<IPerk>.Equals(IPerk? x, IPerk? y) {
    if (x is null || y is null) return false;
    return x.PerkId == y.PerkId;
  }

  int IEqualityComparer<IPerk>.GetHashCode(IPerk obj) {
    return obj.PerkId.GetHashCode();
  }
}

public interface IPerk<K, T> : IPerk {
  /// <summary>
  /// A key identifying an instance of the perk.
  /// </summary>
  K Key { get; init; }

  /// <summary>
  /// The value of the perk.
  /// </summary>
  T Value { get; set; }
}