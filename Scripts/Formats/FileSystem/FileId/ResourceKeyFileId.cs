namespace MySimsToolkit.Scripts.Formats.FileSystem.FileId;

public record ResourceKeyFileId(ResourceKey Key) : IFileSystem.FileId, IHasResourceKey
{
    public override string ToString()
    {
        return Key.ToString();
    }
};