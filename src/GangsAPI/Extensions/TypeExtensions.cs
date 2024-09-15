namespace GangsAPI.Extensions;

public static class TypeExtensions {
  /// <summary>
  ///   Determines if a type is basically primitive. (i.e: SQL-able)
  /// </summary>
  /// <param name="type"></param>
  /// <returns></returns>
  public static bool IsBasicallyPrimitive(this Type type) {
    return type.IsPrimitive || type == typeof(string) || type.IsEnum;
  }
}