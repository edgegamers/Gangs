namespace GangsAPI;

public interface IBehavior : IDisposable {
  void Start();
  void IDisposable.Dispose() { }
}