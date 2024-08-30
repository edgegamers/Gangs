namespace GangsAPI.Struct;

public interface IPerk {
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