using System.Collections.Generic;
using MySimsToolkit.Scripts.Formats.FileSystem;

namespace MySimsToolkit.Scripts.Formats.Material;

public interface IMaterialSet
{
    public List<IFileSystem.FileId> Materials { get; }
}