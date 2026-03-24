using System;
using System.Collections.Generic;
using System.IO;
using MySimsToolkit.Scripts.Formats.FileSystem.FileId;

namespace MySimsToolkit.Scripts.Formats.FileSystem;

public class DdfFileSystem : IFileSystem, IHasRootPath
{
    public string RootPath { get; }
    public readonly Dictionary<ResourceKey, string> Paths = [];

    public DdfFileSystem(string path, Dictionary<string, uint> resourceTypes = null)
    {
        RootPath = path;

        foreach (var filePath in Directory.EnumerateFiles(path))
        {
            Paths.Add(ResourceKey.FromPath(filePath, resourceTypes), filePath);
        }
    }
    
    public bool Exists(IFileSystem.FileId id)
    {
        if (id is IHasResourceKey keyFileId)
        {
            return Paths.ContainsKey(keyFileId.Key);
        }

        return false;
    }

    public Stream OpenRead(IFileSystem.FileId id)
    {
        if (id is not IHasResourceKey keyFileId)
            throw new NotSupportedException("DDFS requires a ResourceKey");

        if (Paths.TryGetValue(keyFileId.Key, out var path))
            return File.OpenRead(path);

        throw new FileNotFoundException($"File not found for key {keyFileId}");
    }

    public Stream OpenWrite(IFileSystem.FileId id)
    {
        if (id is not IHasResourceKey keyFileId)
            throw new NotSupportedException("DDFS requires a ResourceKey");

        if (Paths.TryGetValue(keyFileId.Key, out var path))
            return File.OpenWrite(path);

        throw new FileNotFoundException($"File not found for key {keyFileId}");
    }

    public IEnumerable<IFileSystem.FileId> Enumerate()
    {
        foreach (var key in Paths.Keys)
        {
            yield return new ResourceKeyFileId(key);
        }
    }

    public IFileSystem.FileStat Stat(IFileSystem.FileId id)
    {
        if (id is not IHasResourceKey keyFileId)
            throw new NotSupportedException("DDFS requires a ResourceKey");

        if (Paths.TryGetValue(keyFileId.Key, out var path))
        {
            var fileInfo = new FileInfo(path);
            return new IFileSystem.FileStat(fileInfo.Length, keyFileId.Key.Type);
        }

        throw new FileNotFoundException($"File not found for key {keyFileId}");
    }
}