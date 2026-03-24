using System.Collections.Generic;

namespace MySimsToolkit.Scripts.Formats.FileSystem.FileId;

public record PathFileId(string Path, Dictionary<string, uint> ResourceTypes = null) : IFileSystem.FileId, IHasResourceKey
{
    public ResourceKey Key => ResourceKey.FromPath(Path, ResourceTypes);

    public override string ToString()
    {
        return Path;
    }
};