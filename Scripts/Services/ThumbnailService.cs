using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Godot;
using MySimsToolkit.Scripts.AssetFileTypes;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Nodes;

namespace MySimsToolkit.Scripts.Services;

public class ThumbnailService
{
    public static ThumbnailService Instance => RuntimeRoot.Instance.Runtime.Thumbnails;

    public Dictionary<IFileSystem.FileId, Texture2D> Thumbnails = [];
    public Dictionary<FileType, Func<ThumbnailWorld, IFileSystem.FileId, Task>> Handlers = [];

    private readonly HashSet<IFileSystem.FileId> _inProgress = [];
    private readonly SemaphoreSlim _generationLock = new(1, 1);

    public ThumbnailWorld World;

    public bool HasThumbnail(IFileSystem.FileId path)
    {
        return Thumbnails.ContainsKey(path);
    }

    public Texture2D GetThumbnail(IFileSystem.FileId path)
    {
        return Thumbnails[path];
    }

    public bool ShouldCreateThumbnail(FileType filetype, IFileSystem.FileId path)
    {
        return !HasThumbnail(path)
               && !_inProgress.Contains(path)
               && Handlers.ContainsKey(filetype);
    }

    public void RequestThumbnail(FileType filetype, IFileSystem.FileId path)
    {
        if (!ShouldCreateThumbnail(filetype, path))
            return;

        _ = _RequestThumbnail(filetype, path);
    }
    

    private async Task _RequestThumbnail(FileType filetype, IFileSystem.FileId path)
    {
        _inProgress.Add(path);

        try
        {
            await _generationLock.WaitAsync();

            try
            {
                var generationTask = GenerateThumbnail(filetype, path);
                var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));

                var completed = await Task.WhenAny(generationTask, timeoutTask);

                if (completed == generationTask)
                {
                    await generationTask;
                }
                else
                {
                    GD.Print($"Thumbnail generation timed out: {path}");
                    World.Clear();
                }
            }
            finally
            {
                _generationLock.Release();
            }
        }
        finally
        {
            _inProgress.Remove(path);
        }
    }

    private async Task GenerateThumbnail(FileType filetype, IFileSystem.FileId path)
    {
        await Handlers[filetype](World, path);

        var texture = await World.MakeScreenshot();

        Thumbnails[path] = texture;

        World.Clear();
    }
}