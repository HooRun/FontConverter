namespace FontConverter.Blazor.Services;

public class GlyphRenderQueueService
{
    private readonly Queue<int> _visibleQueue = new();
    private readonly HashSet<int> _inQueue = new();
    private readonly object _lock = new();

    public event Action<int>? OnRenderAllowed;

    private bool _isProcessing = false;
    private readonly int _delayMilliseconds = 10;

    public void EnqueueVisibleGlyph(int trackingId)
    {

        if (_inQueue.Contains(trackingId)) return;

        _visibleQueue.Enqueue(trackingId);
        _inQueue.Add(trackingId);


        _ = ProcessQueueAsync();
    }

    private async Task ProcessQueueAsync()
    {

        if (_isProcessing)
            return;

        _isProcessing = true;


        try
        {
            while (true)
            {
                int trackingId;



                if (_visibleQueue.Count == 0)
                {
                    _isProcessing = false;
                    return;
                }

                trackingId = _visibleQueue.Dequeue();
                _inQueue.Remove(trackingId);


                var handler = OnRenderAllowed;
                handler?.Invoke(trackingId);

                await Task.Delay(_delayMilliseconds);
            }
        }
        finally
        {

            _isProcessing = false;

        }
    }


    public void UnregisterGlyph(int trackingId)
    {

        var filteredQueue = _visibleQueue
            .Where(id => id != trackingId)
            .ToList();

        _visibleQueue.Clear();
        foreach (var id in filteredQueue)
            _visibleQueue.Enqueue(id);

        _inQueue.Remove(trackingId);

    }

    public void ClearAll()
    {

        _visibleQueue.Clear();
        _inQueue.Clear();

    }

}