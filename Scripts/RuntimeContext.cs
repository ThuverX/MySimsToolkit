using System;
using Godot;
using MySimsToolkit.Scripts.Platforms;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts;

public class RuntimeContext : IDisposable
{
    
    public JobService Jobs { get; }
    public FileSystemService FileSystem { get; set; }
    public AssetService Assets { get; }
    public ShaderService Shaders { get; }
    public ThumbnailService Thumbnails { get; }
    public GamePlatform Platform { get; }
    public FileTranslatorService Translator { get; }
    public SaveService Saves { get; }
    
    public RuntimeContext(GamePlatform platform, string rootPath)
    {
        Platform = platform;
        
        Jobs = new JobService();
        Translator = new FileTranslatorService();
        FileSystem = new FileSystemService(rootPath);
        Assets = new AssetService();
        Shaders = new ShaderService();
        Thumbnails = new ThumbnailService();
        Saves = new SaveService();

        Platform.RegisterTranslator(Translator);
        Platform.MountFileSystems(FileSystem);
        Platform.RegisterAssetLoaders(Assets);
        Platform.RegisterAssetTypes(FileSystem);
        Platform.RegisterThumbnailHandlers(Thumbnails);
        Platform.MountSaveFileSystems(Saves);
        
        GD.Print("Runtime for ", Platform.GameType, " Ready!");
    }

    public void Dispose()
    {
        Jobs.Dispose();
        Shaders.Dispose();
    }
}