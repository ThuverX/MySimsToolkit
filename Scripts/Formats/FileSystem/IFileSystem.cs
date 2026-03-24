using System.Collections.Generic;
using System.IO;

namespace MySimsToolkit.Scripts.Formats.FileSystem;

public interface IFileSystem
{
    public abstract record FileId;
    public record FileStat(long Size, uint Type);

    bool Exists(FileId id);
    Stream OpenRead(FileId id);
    Stream OpenWrite(FileId id);
    IEnumerable<FileId> Enumerate();
    FileStat Stat(FileId id);
}