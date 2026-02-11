public interface IPausable
{
    public bool IsPaused { get; }
    public void SetState(bool state);
}