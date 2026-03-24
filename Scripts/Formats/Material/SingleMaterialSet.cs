using System.Collections.Generic;
using MySimsToolkit.Scripts.Formats.FileSystem;

namespace MySimsToolkit.Scripts.Formats.Material;

public class SingleMaterialSet(IFileSystem.FileId material) : IMaterialSet
{
    public List<IFileSystem.FileId> Materials => [material];
}