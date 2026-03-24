using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.Platforms;

public class NonePlatform : GamePlatform
{
    public override GameType GameType => GameType.None;
    public override bool HasSaveLocation => false;
    public override bool IsValidPath(string path)
    {
        return true;
    }

    public override void MountFileSystems(FileSystemService fileSystem)
    {
    }

    public override void RegisterAssetLoaders(AssetService assets)
    {
    }

    public override void RegisterAssetTypes(FileSystemService fileSystem)
    {
    }

    public override void RegisterThumbnailHandlers(ThumbnailService thumbnails)
    {
    }

    public override void MountSaveFileSystems(SaveService saves)
    {
    }
}