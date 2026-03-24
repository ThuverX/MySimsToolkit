using System.Collections.Generic;
using MySimsToolkit.Scripts.Formats.FileSystem;

namespace MySimsToolkit.Scripts.Formats.Level;

public interface ILevel
{
    public List<IFileSystem.FileId> Parts { get; }
    public List<ObjectInfo>  Objects { get; }
}