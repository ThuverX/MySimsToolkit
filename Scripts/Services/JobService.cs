using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MySimsToolkit.Scripts.Nodes;

namespace MySimsToolkit.Scripts.Services;

public class JobService : IDisposable
{
    public static JobService Instance => RuntimeRoot.Instance.Runtime.Jobs;
    
    
    private readonly BlockingCollection<Action> _queue = new();
    private readonly ConcurrentQueue<Action> _mainThreadQueue = new();
    private readonly Thread _thread;
    
    public JobService()
    {
        _thread = new Thread(WorkerLoop)
        {
            IsBackground = true,
            Name = "WorkerThread"
        };
        _thread.Start();
    }
    
    public Task<T> Run<T>(Func<T> work)
    {
        var tcs = new TaskCompletionSource<T>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        _queue.Add(() =>
        {
            try
            {
                var result = work();
                tcs.SetResult(result);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });

        return tcs.Task;
    }
    
    public Task<T> Run<T>(Func<Task<T>> work)
    {
        var tcs = new TaskCompletionSource<T>(
            TaskCreationOptions.None);

        _queue.Add(async void () =>
        {
            try
            {
                var result = await work().ConfigureAwait(false);
                tcs.SetResult(result);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });

        return tcs.Task;
    }

    
    public Task SwitchToMainThread()
    {
        var tcs = new TaskCompletionSource();

        _mainThreadQueue.Enqueue(() => tcs.SetResult());

        return tcs.Task;
    }

    public Task RunOnMainThread(Action action)
    {
        var tcs = new TaskCompletionSource(
            TaskCreationOptions.RunContinuationsAsynchronously);

        _mainThreadQueue.Enqueue(() =>
        {
            try
            {
                action();
                tcs.SetResult();
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }
        });

        return tcs.Task;
    }
    
    public void ExecuteMainThreadJobs()
    {
        while (_mainThreadQueue.TryDequeue(out var action))
            action();
    }
    
    private void WorkerLoop()
    {

        foreach (var job in _queue.GetConsumingEnumerable())
        {
            job();
        }
    }

    public void Dispose()
    {
        _queue.CompleteAdding();
        _thread.Join();
        _queue.Dispose();
    }
}