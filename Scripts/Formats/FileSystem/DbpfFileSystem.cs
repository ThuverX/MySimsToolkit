using System;
using System.Collections.Generic;
using System.IO;
using MySimsToolkit.Scripts.Formats.DBPF;
using MySimsToolkit.Scripts.Formats.FileSystem.FileId;

namespace MySimsToolkit.Scripts.Formats.FileSystem;

public class DbpfFileSystem : IFileSystem, IHasRootPath
{
    public string RootPath { get; }
    
    private DbpfFile _file;
    
    public DbpfFileSystem(string path, EndiannessAwareBinaryReader.Endianness endianness = EndiannessAwareBinaryReader.Endianness.Little)
    {
        RootPath = path;

        using var stream = File.OpenRead(RootPath);
        using var reader = new EndiannessAwareBinaryReader(stream, endianness);
        
        _file = DbpfFile.Read(reader);
    }

    public bool Exists(IFileSystem.FileId id)
    {
        if (id is IHasResourceKey keyFileId)
        {
            return _file.Entries.ContainsKey(keyFileId.Key);
        }

        return false;
    }

    public Stream OpenRead(IFileSystem.FileId id)
    {
        if (id is not IHasResourceKey keyFileId)
            throw new NotSupportedException("DBPF requires a ResourceKey");

        if (_file.Entries.TryGetValue(keyFileId.Key, out var entry))
        {
            return new SubStream(File.OpenRead(RootPath), entry.Offset, entry.Size);
        }

        throw new FileNotFoundException($"File not found for key {keyFileId}");
    }

    public Stream OpenWrite(IFileSystem.FileId id)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IFileSystem.FileId> Enumerate()
    {
        foreach (var key in _file.Entries.Keys)
        {
            yield return new ResourceKeyFileId(key);
        }
    }

    public IFileSystem.FileStat Stat(IFileSystem.FileId id)
    {
        if (id is not IHasResourceKey keyFileId)
            throw new NotSupportedException("DBPF requires a ResourceKey");

        if (_file.Entries.TryGetValue(keyFileId.Key, out var entry))
            return new IFileSystem.FileStat(entry.Size, entry.Key.Type);

        throw new FileNotFoundException($"File not found for key {keyFileId}");
    }

}