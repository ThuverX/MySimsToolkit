using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MySimsToolkit.Scripts.Formats.FileSystem;

public class VirtualFileSystem : IFileSystem
{
    private readonly List<IFileSystem> _mounts = [];
    
    public List<IFileSystem> Mounts => _mounts;
    
    public bool Exists(IFileSystem.FileId id)
    {
        return _mounts.Any(fileSystem => fileSystem.Exists(id));
    }

    public Stream OpenRead(IFileSystem.FileId id)
    {
        foreach (var fs in _mounts)
        {
            if (fs.Exists(id))
                return fs.OpenRead(id);
        }
        
        throw new FileNotFoundException($"File not found for key {id}");
    }

    public Stream OpenWrite(IFileSystem.FileId id)
    {
        foreach (var fs in _mounts)
        {
            if (fs.Exists(id))
                return fs.OpenWrite(id);
        }
        
        throw new FileNotFoundException($"File not found for key {id}");
    }

    public IEnumerable<IFileSystem.FileId> Enumerate()
    {
        HashSet<IFileSystem.FileId> seen = [];

        foreach (var fs in _mounts)
        {
            foreach (var id in fs.Enumerate())
            {
                if (seen.Add(id))
                    yield return id;
            }
        }
    }

    public IFileSystem.FileStat Stat(IFileSystem.FileId id)
    {
        foreach (var fs in _mounts)
        {
            if (fs.Exists(id))
                return fs.Stat(id);
        }
        
        throw new FileNotFoundException($"File not found for key {id}");
    }
    
    public void Mount(IFileSystem fs)
    {
        _mounts.Insert(0, fs);
    }

    public void MountLast(IFileSystem fs)
    {
        _mounts.Add(fs);
    }
}