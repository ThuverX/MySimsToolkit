using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Godot;
using MySimsToolkit.Scripts.AssetFileTypes;
using MySimsToolkit.Scripts.AssetLoaders;
using MySimsToolkit.Scripts.Extensions;
using MySimsToolkit.Scripts.Formats;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Formats.Level;
using MySimsToolkit.Scripts.Formats.Material;
using MySimsToolkit.Scripts.Formats.Model;
using MySimsToolkit.Scripts.Services;
using MySimsToolkit.Scripts.ThumbnailHandlers;

namespace MySimsToolkit.Scripts.Platforms;

public class MySimsPcPlatform : GamePlatform
{
    public override GameType GameType => GameType.MySimsPc;
    public override bool HasSaveLocation => false;
    
    public Dictionary<string, uint> ResourceTypes = [];
    
    public override bool IsValidPath(string path)
    {
        return File.Exists(Path.Join(path, "MySims.exe")) && Directory.Exists(Path.Join(path, "../", "SimsRevData"));
    }

    public override void MountFileSystems(FileSystemService fileSystem)
    {
        var rootPath = fileSystem.RootPath;
        MountFolder(fileSystem, rootPath);

        LoadDdfMap(rootPath);
        
        foreach (var directory in Directory.EnumerateDirectories(Path.Join(rootPath,  "../", "SimsRevData"), "",
                     SearchOption.AllDirectories))
        {
            MountFolder(fileSystem, directory);
        }
        
        foreach (var enumerateFile in Directory.EnumerateFiles(Path.Join(rootPath, "../", "SimsRevData"), "*.package", SearchOption.AllDirectories))
        {
             fileSystem.MountLast(new DbpfFileSystem(enumerateFile));
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

    private void LoadDdfMap(string rootPath)
    {
        ResourceTypes = [];
        var mapPath = Path.Join(rootPath, "../", "SimsRevData", "GameData", "DDFMap.txt");
        if (!File.Exists(mapPath)) return;

        var lines = File.ReadAllLines(mapPath);
        foreach (var line in lines)
        {
            if (line.StartsWith("//")) continue;
            var split = line.Split(' ');
            if (split.Length != 2) continue;

            var type = split[0][2..];
            var extension = split[1].ToLower();

            ResourceTypes.TryAdd(
                extension,
                Convert.ToUInt32(type, 16)
            );
        }
    }

    public override void RegisterAssetLoaders(AssetService assets)
    {
        assets.RegisterLoader(BinaryLoader.Create<IModel, WindowsModel>());
        assets.RegisterLoader(StreamLoader.Create<ILevel, LevelXml>());
        assets.RegisterLoader(TextureLoader.Create(TextureLoader.ImageType.Dds));
        assets.RegisterLoader(MaterialLoader);
        assets.RegisterLoader(MaterialSetLoader);
    }
    
    public override void RegisterAssetTypes(FileSystemService fileSystem)
    {
        fileSystem.FileTypes.AddRange([
            new DdsType(),
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
        thumbnails.Handlers.Add(new LevelType(), Generic3DHandler.LevelHandler);
        thumbnails.Handlers.Add(new WindowsModelType(), Generic3DHandler.ModelHandler);
        thumbnails.Handlers.Add(new DdsType(), Generic3DHandler.TextureHandler);
    }

    public override void MountSaveFileSystems(SaveService saves)
    {
    }

    private Task<IMaterial> MaterialLoader(IFileSystem.FileId fileId)
    {
        return JobService.Instance.Run(IMaterial () =>
        {
            using var stream = FileSystemService.Instance.OpenRead(fileId);
            using var reader = new EndiannessAwareBinaryReader(stream);
            return MaterialData.Read(reader, MaterialVersion.MySims);
        });
    }

    private Task<IMaterialSet> MaterialSetLoader(IFileSystem.FileId fileId)
    {
        return JobService.Instance.Run(IMaterialSet () =>
        {
            var type = FileSystemService.Instance.Stat(fileId).Type;

            if (type == ResourceTypes["material"])
            {
                return new SingleMaterialSet(fileId);
            }

            if (type == ResourceTypes["materialset"])
            {
                using var stream = FileSystemService.Instance.OpenRead(fileId);
                using var reader = new EndiannessAwareBinaryReader(stream);

                return MaterialSet.Read(reader, MaterialVersion.MySims);
            }
            
            throw new FileNotFoundException($"File not found for key {fileId}");
        });
    }
}