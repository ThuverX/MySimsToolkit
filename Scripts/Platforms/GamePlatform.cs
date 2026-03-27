using System;
using System.IO;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.Platforms;

public abstract class GamePlatform
{
    protected FileTranslatorService FileTranslator;
    public abstract GameType GameType { get; }
    public virtual string SaveLocation { get; } = "";
    public abstract bool HasSaveLocation { get; }

    public static GameType DetectPlatform(string path)
    {
        var folderPath = Path.GetDirectoryName(path);
        
        if(CheckPlatform<MySimsCozyBundlePcPlatform>(folderPath, out var mySimsCozyBundlePcPlatform)) return mySimsCozyBundlePcPlatform;
        if(CheckPlatform<MySimsKingdomCozyBundlePcPlatform>(folderPath, out var mySimsKingdomCozyBundlePcPlatform)) return mySimsKingdomCozyBundlePcPlatform;
        if(CheckPlatform<MySimsPcPlatform>(folderPath, out var mySimsPcPlatform)) return mySimsPcPlatform;
        if(CheckPlatform<MySimsAgentsWiiPlatform>(folderPath, out var mySimsAgentsWiiPlatform)) return mySimsAgentsWiiPlatform;

        return GameType.None;
    }

    private static bool CheckPlatform<T>(string path, out GameType type) where T : GamePlatform, new()
    {
        var platform = new T();
        if (platform.IsValidPath(path))
        {
            type = platform.GameType;
            return true;
        }
        type = GameType.None;
        return false;
    }

    public static GamePlatform FromType(GameType gameType)
    {
        switch (gameType)
        {
            case GameType.MySimsPc:
                return new MySimsPcPlatform();
            case GameType.MySimsDs:
                break;
            case GameType.MySimsCozyBundlePc:
                return new MySimsCozyBundlePcPlatform();
            case GameType.MySimsKingdomCozyBundlePc:
                return new MySimsKingdomCozyBundlePcPlatform();
            case GameType.MySimsPartyWii:
                break;
            case GameType.MySimsRacingWii:
                break;
            case GameType.MySimsAgentsWii:
                return new MySimsAgentsWiiPlatform();
            case GameType.MySimsAgentsDs:
                break;
            case GameType.MySimsSkyHeroes:
                break;
            case GameType.None:
                return new NonePlatform();
            default:
                throw new ArgumentOutOfRangeException(nameof(gameType), gameType, null);
        }
        return null;
    }
    
    public abstract bool IsValidPath(string path);
    public abstract void MountFileSystems(FileSystemService fileSystem);
    public abstract void RegisterAssetLoaders(AssetService assets);
    public abstract void RegisterAssetExporters(ExporterService assets);
    public abstract void RegisterAssetTypes(FileSystemService fileSystem);
    

    public void RegisterTranslator(FileTranslatorService translator)
    {
        FileTranslator = translator;
    }

    public abstract void RegisterThumbnailHandlers(ThumbnailService thumbnails);
    public abstract void MountSaveFileSystems(SaveService saves);
}