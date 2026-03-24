using System.Collections.Generic;
using System.IO;
using MySimsToolkit.Scripts.AssetFileTypes;
using MySimsToolkit.Scripts.AssetLoaders;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Services;
using MySimsToolkit.Scripts.ThumbnailHandlers;

namespace MySimsToolkit.Scripts.Platforms;

public class MySimsAgentsWiiPlatform : GamePlatform
{
    public override GameType GameType => GameType.MySimsAgentsWii;
    public override bool HasSaveLocation => false;

    public static Dictionary<string, uint> ResourceTypes = new()
     {
         { "material", 0xE6640542 },
         { "materialset", 0x787E842A },
     };
    
    public override bool IsValidPath(string path)
    {
        return File.Exists(Path.Join(path, "opening.bnr")) && Directory.Exists(Path.Join(path, "packages"));
    }

    public override void MountFileSystems(FileSystemService fileSystem)
    {
        var rootPath = fileSystem.RootPath;

        MountFolder(fileSystem, rootPath);
        
        foreach (var directory in Directory.EnumerateDirectories(Path.Join(rootPath), "",
                     SearchOption.AllDirectories))
        {
            MountFolder(fileSystem, directory);
        }
        
        foreach (var enumerateFile in Directory.EnumerateFiles(Path.Join(rootPath, "Packages"), "*.package", SearchOption.AllDirectories))
        {
            fileSystem.MountLast(new DbpfFileSystem(enumerateFile, EndiannessAwareBinaryReader.Endianness.Big));
        }
    }
    
    private void MountFolder(VirtualFileSystem fileSystem, string path)
    {
        var ddf = new DdfFileSystem(path, ResourceTypes);

        foreach (var keyValuePair in ddf.Paths)
        {
            FileTranslator.AddTranslation(keyValuePair.Key, Path.GetFileName(keyValuePair.Value));
        }

        fileSystem.MountLast(ddf);
    }

    public override void RegisterAssetLoaders(AssetService assets)
    {
        assets.RegisterLoader(TextureLoader.Create(TextureLoader.ImageType.Tpl));
    }

    public override void RegisterAssetTypes(FileSystemService fileSystem)
    {
        fileSystem.FileTypes.AddRange([
            new TplType(),
            new DynamicLinkLibraryType(),
            new ExecutableType(),
            new FontType(),
            new GenericType(),
            new LevelType(),
            new MaterialSetType(),
            new MaterialType(),
            new PackageType(),
            new ShaderType(),
            new TextType(),
            new WindowsModelType(),
            new WorldType(),
            new XmlType()
        ]);
    }

    public override void RegisterThumbnailHandlers(ThumbnailService thumbnails)
    {
        thumbnails.Handlers.Add(new TplType(), Generic3DHandler.TextureHandler);
    }

    public override void MountSaveFileSystems(SaveService saves)
    {
        throw new System.NotImplementedException();
    }
}