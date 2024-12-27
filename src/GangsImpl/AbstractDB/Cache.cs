using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace GenericDB;

public class Cache<TKey, TValue>(TimeSpan expiration)
  : IDictionary<TKey, TValue> where TKey : notnull {
  private readonly Dictionary<TKey, CacheItem<TValue?>> cache = new();

  public void Add(TKey key, TValue value) {
    cache[key] = new CacheItem<TValue?>(value, expiration);
  }

  public bool ContainsKey(TKey key) {
    return cache.TryGetValue(key, out var cached)
      && DateTimeOffset.Now - cached.Created < cached.ExpiresAfter;
  }

  public bool Remove(TKey key) { return cache.Remove(key); }

  public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
    if (!cache.TryGetValue(key, out var cached)) {
      value = default;
      return false;
    }

    if (cached.IsValid) {
      value = cached.Value!;
      return true;
    }

    cache.Remove(key);
    value = default;
    return false;
  }

  public TValue this[TKey key] {
    get {
      if (!cache.TryGetValue(key, out var cached))
        throw new KeyNotFoundException();
      if (!cached.IsValid) throw new KeyNotFoundException();
      return cached.Value!;
    }
    set => cache[key] = new CacheItem<TValue?>(value, expiration);
  }

  public ICollection<TKey> Keys
    => cache.Keys.Where(key
        => DateTimeOffset.Now - cache[key].Created < cache[key].ExpiresAfter)
     .ToList();

  public ICollection<TValue> Values
    => cache.Values.Where(cached => cached.IsValid && cached.Value != null)
     .Select(cached => cached.Value!)
     .ToList();

  public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
    return cache.Where(kv => kv.Value.IsValid)
     .Where(kv => kv.Value.Value != null)
     .Select(kv => new KeyValuePair<TKey, TValue>(kv.Key, kv.Value.Value!))
     .GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }

  public void Add(KeyValuePair<TKey, TValue> item) {
    cache[item.Key] = new CacheItem<TValue?>(item.Value, expiration);
  }

  public void Clear() { cache.Clear(); }

  public bool Contains(KeyValuePair<TKey, TValue> item) {
    return cache.TryGetValue(item.Key, out var cached) && cached.IsValid
      && EqualityComparer<TValue>.Default.Equals(cached.Value!, item.Value);
  }

  public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
    foreach (var kv in cache.Where(kv => kv.Value.IsValid)) {
      array[arrayIndex++] =
        new KeyValuePair<TKey, TValue>(kv.Key, kv.Value.Value!);
    }
  }

  public bool Remove(KeyValuePair<TKey, TValue> item) {
    return cache.Remove(item.Key);
  }

  public int Count
    => cache.Count(kv => kv.Value.IsValid && kv.Value.Value != null);

  public bool IsReadOnly => false;
}

public class CacheItem<T>(T value, TimeSpan expiresAfter) {
  public T Value { get; } = value;
  internal DateTimeOffset Created { get; } = DateTimeOffset.Now;
  internal TimeSpan ExpiresAfter { get; } = expiresAfter;

  public bool IsValid => DateTimeOffset.Now - Created < ExpiresAfter;
}