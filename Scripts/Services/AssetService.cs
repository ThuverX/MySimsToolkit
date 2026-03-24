using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Nodes;

namespace MySimsToolkit.Scripts.Services;

public class AssetService
{
    public static AssetService Instance => RuntimeRoot.Instance.Runtime.Assets;
    
    public sealed record AssetHandle<T>
    {
        private int _refCount = 1;

        internal Task<T> Task => Tcs.Task;
        internal readonly TaskCompletionSource<T> Tcs =
            new(TaskCreationOptions.RunContinuationsAsynchronously);

        public AssetState State { get; private set; } = AssetState.Loading;
        public T Value { get; private set; }
        public Exception Error { get; private set; }

        public event Action<AssetHandle<T>> Completed;
        internal Action OnReleased;

        internal void SetResult(T value)
        {
            if (State != AssetState.Loading) return;

            Value = value;
            State = AssetState.Ready;
            Tcs.SetResult(value);
            Completed?.Invoke(this);
        }

        internal void SetFailed(Exception ex)
        {
            if (State != AssetState.Loading) return;

            Error = ex;
            State = AssetState.Failed;
            Tcs.SetException(ex);
            Completed?.Invoke(this);
        }

        public AssetHandle<T> Retain()
        {
            Interlocked.Increment(ref _refCount);
            return this;
        }

        public void Release()
        {
            if (Interlocked.Decrement(ref _refCount) == 0)
            {
                OnZeroReferences();
            }
        }

        private void OnZeroReferences()
        {
            State = AssetState.Unloaded;
            OnReleased?.Invoke();
        }

        public Task<T> AsTask() => Task;
    }
    
    public readonly record struct AssetKey(Type Type, IFileSystem.FileId FileId);
    
    private readonly Dictionary<AssetKey, object> _assets = new();

    private readonly Dictionary<Type, Func<IFileSystem.FileId, Task<object>>> _loaders = new();

    public void RegisterLoader<T>(Func<IFileSystem.FileId, Task<T>> loader)
    {
        _loaders[typeof(T)] = async id => await loader(id);
    }
    
    public AssetHandle<T> Load<T>(IFileSystem.FileId id)
    {
        var key = new AssetKey(typeof(T), id);

        if (_assets.TryGetValue(key, out var existing))
        {
            return ((AssetHandle<T>)existing).Retain();
        }

        if (!_loaders.TryGetValue(typeof(T), out var loader))
            throw new Exception($"No loader registered for {typeof(T).Name}");

        var handle = new AssetHandle<T>();
        _assets[key] = handle;
        handle.OnReleased = () => _assets.Remove(key);

        StartLoad(handle, id, loader);

        return handle;
    }
    
    private void StartLoad<T>(
        AssetHandle<T> handle,
        IFileSystem.FileId id,
        Func<IFileSystem.FileId, Task<object>> loader)
    {
        
        _ = JobService.Instance.Run(async () =>
        {
            try
            {
                var obj = await loader(id);
                handle.SetResult((T)obj);
            }
            catch (Exception ex)
            {
                handle.SetFailed(ex);
            }
        });
    }

    public enum AssetState
    {
        Loading,
        Ready,
        Failed,
        Unloaded
    }
}