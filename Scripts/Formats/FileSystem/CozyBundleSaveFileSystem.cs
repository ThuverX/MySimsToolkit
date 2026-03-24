using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using MySimsToolkit.Scripts.Formats.FileSystem.FileId;
using MySimsToolkit.Scripts.Formats.Save;

namespace MySimsToolkit.Scripts.Formats.FileSystem;

public class CozyBundleSaveFileSystem : IFileSystem, IHasRootPath
{
    public string RootPath { get; }
    
    private readonly CozyBundleSave _file;
    
    public CozyBundleSaveFileSystem(string path, EndiannessAwareBinaryReader.Endianness endianness = EndiannessAwareBinaryReader.Endianness.Little)
    {
        RootPath = path;

        using var stream = File.OpenRead(RootPath);
        using var reader = new EndiannessAwareBinaryReader(stream, endianness);
        
        _file = CozyBundleSave.Read(reader);
    }
    
    public bool Exists(IFileSystem.FileId id)
    {
        return _file.Entries.ContainsKey(id.ToString());
    }

    public Stream OpenRead(IFileSystem.FileId id)
    {
        if (_file.Entries.TryGetValue(id.ToString(), out var entry))
        {
            return new ZLibStream(new SubStream(File.OpenRead(RootPath), entry.Offset, entry.Size), CompressionMode.Decompress);
        }
        
        throw new FileNotFoundException($"File not found: {id}");
    }

    public Stream OpenWrite(IFileSystem.FileId id)
    {
        throw new System.NotImplementedException();
    }

    public IEnumerable<IFileSystem.FileId> Enumerate()
    {
        foreach (var keyValuePair in _file.Entries)
        {
            yield return new PathFileId(keyValuePair.Key);
        }
    }

    public IFileSystem.FileStat Stat(IFileSystem.FileId id)
    {
        if (_file.Entries.TryGetValue(id.ToString(), out var entry))
        {
            // TODO: is this right?
            return new IFileSystem.FileStat(entry.Size, 0);
        }
        
        throw new FileNotFoundException($"File not found: {id}");
    }
}