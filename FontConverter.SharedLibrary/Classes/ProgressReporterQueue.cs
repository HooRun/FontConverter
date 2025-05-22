using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using System.Threading.Tasks;

public class ProgressReporterQueue<T> : IDisposable
{
    private readonly Channel<T> _channel;
    private readonly IProgress<T>? _progress;
    private readonly int _delayMs;
    private Task? _consumerTask;
    private CancellationTokenSource? _cts;
    private bool _disposed;
    private double _lastReportedPercentage = -1;

    public ProgressReporterQueue([AllowNull] IProgress<T> progress = null, int delayMilliseconds = 500)
    {
        _channel = Channel.CreateBounded<T>(new BoundedChannelOptions(100)
        {
            SingleReader = true,
            AllowSynchronousContinuations = false,
            FullMode = BoundedChannelFullMode.Wait
        });
        _progress = progress;
        _delayMs = delayMilliseconds;
    }

    public void Start()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ProgressReporterQueue<T>));
        _cts = new CancellationTokenSource();
        _consumerTask = Task.Run(() => ConsumeAsync(_cts.Token));
    }

    public async Task StopAsync()
    {
        if (_disposed)
            return;
        try
        {
            _channel.Writer.Complete();
            if (_consumerTask != null)
                await _consumerTask.ConfigureAwait(false);
        }
        finally
        {
            _cts?.Dispose();
            _cts = null;
        }
    }

    public void Enqueue(T item)
    {
        if (_disposed)
            return;
        if (item is ValueTuple<string, double> progress && Math.Abs(progress.Item2 - _lastReportedPercentage) < 1)
            return;
        if (_channel.Writer.TryWrite(item))
        {
            if (item is ValueTuple<string, double> progressItem)
                _lastReportedPercentage = progressItem.Item2;
        }
    }

    private async Task ConsumeAsync(CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var item in _channel.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
            {
                try
                {
                    if (_progress != null)
                        _progress.Report(item);
                    if (_delayMs > 0)
                        await Task.Delay(_delayMs, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public void Dispose()
    {
        if (_disposed)
            return;
        //_cts?.Dispose();
        _disposed = true;
    }
}