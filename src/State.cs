namespace Kube.Demo;

sealed class State
{
    public State()
    {
        Ready = true;
        Alive = true;
    }
    
    /// <summary>
    /// Mutex Lock
    /// </summary>
    public object Lock { get; } = new();
    
    /// <summary>
    /// App is ready
    /// </summary>
    public bool Ready { get; set; }
    
    /// <summary>
    /// App is alive
    /// </summary>
    public bool Alive { get; set; }
}