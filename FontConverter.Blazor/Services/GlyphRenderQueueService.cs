using System.Collections.Concurrent;
using System.Threading;

namespace FontConverter.Blazor.Services;

public class GlyphRenderQueueService
{
    private readonly ConcurrentQueue<int> _visibleQueue = new();
    private readonly ConcurrentDictionary<int, byte> _inQueue = new();

    public event Action<int>? OnRenderAllowed;

    private int _isProcessing = 0;
    private readonly int _delayMilliseconds = 0;
    private readonly int _batchSize = 20;

    public void EnqueueVisibleGlyph(int trackingId)
    {
        if (!_inQueue.TryAdd(trackingId, 0))
            return;

        _visibleQueue.Enqueue(trackingId);
        _ = ProcessQueueAsync();
    }

    private async Task ProcessQueueAsync()
    {
        if (Interlocked.CompareExchange(ref _isProcessing, 1, 0) != 0)
            return;

        try
        {
            while (true)
            {
                if (_visibleQueue.IsEmpty)
                    break;

                int processedCount = 0;

                while (processedCount < _batchSize && _visibleQueue.TryDequeue(out var trackingId))
                {
                    if (!_inQueue.TryRemove(trackingId, out _))
                        continue;

                    OnRenderAllowed?.Invoke(trackingId);
                    processedCount++;
                }

                if (processedCount == 0)
                    break;

                await Task.Delay(_delayMilliseconds);
            }
        }
        finally
        {
            Interlocked.Exchange(ref _isProcessing, 0);

            if (!_visibleQueue.IsEmpty)
                _ = ProcessQueueAsync();
        }
    }

    public void UnregisterGlyph(int trackingId)
    {
        _inQueue.TryRemove(trackingId, out _);
    }

    public void ClearAll()
    {
        while (_visibleQueue.TryDequeue(out _)) { }
        _inQueue.Clear();
    }
}
