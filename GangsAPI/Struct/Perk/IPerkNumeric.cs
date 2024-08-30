using System.Numerics;

namespace GangsAPI.Struct.Perk;

public interface IPerkNumeric<K, T> : IPerk<K, T?> where T : INumber<T?> { }