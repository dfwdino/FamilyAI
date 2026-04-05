namespace FamilyAI.Infrastructure.Services;

/// <summary>
/// Singleton that tracks which threads have an AI response in-flight.
/// Allows the chat UI to restore the typing indicator when navigating back,
/// and to notify the user when a response arrives on a different thread.
/// </summary>
public class PendingAiTracker
{
    private readonly Dictionary<int, string> _pending = new(); // threadId → thread name
    private readonly object _lock = new();

    /// <summary>Fired when an AI response completes. Args: threadId, threadName.</summary>
    public event Action<int, string>? OnAiResponded;

    public void SetPending(int threadId, string threadName)
    {
        lock (_lock) _pending[threadId] = threadName;
    }

    public void SetResponded(int threadId)
    {
        string? threadName;
        lock (_lock)
        {
            _pending.TryGetValue(threadId, out threadName);
            _pending.Remove(threadId);
        }
        if (threadName != null)
            OnAiResponded?.Invoke(threadId, threadName);
    }

    public bool IsPending(int threadId)
    {
        lock (_lock) return _pending.ContainsKey(threadId);
    }
}
