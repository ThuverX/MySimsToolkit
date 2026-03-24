using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.Nodes;

public abstract partial class LoaderNode3D : Node3D
{
    protected readonly List<AssetService.AssetHandle<object>> Assets = [];

    private readonly TaskCompletionSource _fullyLoadedTcs = new();
    public Task WhenFullyLoaded => _fullyLoadedTcs.Task;

    private int _assetsLoading = 0;

    protected AssetService.AssetHandle<T> RequestAsset<T>(IFileSystem.FileId fileId)
    {
        var handle = AssetService.Instance.Load<T>(fileId);

        if (handle.State != AssetService.AssetState.Loading)
        {
            OnAssetHandleLoadComplete(handle);
            return handle;
        }

        _assetsLoading++;

        Assets.Add(handle as AssetService.AssetHandle<object>);
        handle.Completed += CompletionHandler;

        return handle;

        void CompletionHandler(AssetService.AssetHandle<T> _)
        {
            handle.Completed -= CompletionHandler;
            _assetsLoading--;
            OnAssetHandleLoadComplete(handle);
        }
    }

    private void OnAssetHandleLoadComplete<T>(AssetService.AssetHandle<T> handle)
    {
        if (handle.State != AssetService.AssetState.Ready)
        {
            GD.PrintErr("Failed to load asset: " + handle);
            _fullyLoadedTcs.TrySetException(handle.Error ?? new Exception("Unknown asset load failure."));
            return;
        }

        if (_assetsLoading != 0)
            return;

        JobService.Instance.RunOnMainThread(async void () =>
        {
            try
            {
                await OnAssetsLoaded();
                await WaitForChildren();
                _fullyLoadedTcs.TrySetResult();
            }
            catch (Exception e)
            {
                _fullyLoadedTcs.TrySetException(e);
            }
        });
    }

    private async Task WaitForChildren()
    {
        var tasks = new List<Task>();
        foreach (var child in GetChildren())
        {
            if (child is LoaderNode3D loaderNode3D)
            {
                tasks.Add(loaderNode3D.WhenFullyLoaded);
            }
        }
        
        await Task.WhenAll(tasks);
    }

    protected abstract Task OnAssetsLoaded();

    public new void QueueFree()
    {
        base.QueueFree();

        foreach (var assetHandle in Assets)
            assetHandle.Release();

        if (!_fullyLoadedTcs.Task.IsCompleted)
            _fullyLoadedTcs.TrySetCanceled();
    }
}