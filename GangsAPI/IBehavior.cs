namespace GangsAPI;

public interface IBehavior : IDisposable {
  void IDisposable.Dispose() { }
  void Start();
}