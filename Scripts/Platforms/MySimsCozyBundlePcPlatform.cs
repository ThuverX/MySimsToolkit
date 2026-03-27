using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Godot;
using MySimsToolkit.Scripts.AssetExporters;
using MySimsToolkit.Scripts.AssetFileTypes;
using MySimsToolkit.Scripts.AssetLoaders;
using MySimsToolkit.Scripts.Extensions;
using MySimsToolkit.Scripts.Formats;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Formats.Level;
using MySimsToolkit.Scripts.Formats.Material;
using MySimsToolkit.Scripts.Formats.Model;
using MySimsToolkit.Scripts.Saves;
using MySimsToolkit.Scripts.Services;
using MySimsToolkit.Scripts.ThumbnailHandlers;
using Environment = System.Environment;

namespace MySimsToolkit.Scripts.Platforms;

public class MySimsCozyBundlePcPlatform : GamePlatform
{
    public override GameType GameType => GameType.MySimsCozyBundlePc;
    public override bool HasSaveLocation => true;
    public override string SaveLocation => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Electronic Arts",
        "MySims"
    );

    public Dictionary<string, uint> ResourceTypes = [];

    public override bool IsValidPath(string path)
    {
        return File.Exists(Path.Join(path, "MySims.exe")) && Directory.Exists(Path.Join(path, "data"));
    }

    public override void MountFileSystems(FileSystemService fileSystem)
    {
        var rootPath = fileSystem.RootPath;
        MountFolder(fileSystem, rootPath);

        LoadDdfMap(rootPath);
        LoadResourceNames(rootPath);

        foreach (var directory in Directory.EnumerateDirectories(Path.Join(rootPath, "data"), "",
                     SearchOption.AllDirectories))
        {
            MountFolder(fileSystem, directory);
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
        var mapPath = Path.Join(rootPath, "data", "GameData", "DDFMap.txt");
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

    private void LoadResourceNames(string rootPath)
    {
        var assetMapPath = Path.Join(rootPath, "data", "BuildData", "AssetGenerator", "AssetMap.xml");
        if (!File.Exists(assetMapPath)) return;

        var doc = XDocument.Load(assetMapPath);

        var root = doc.Element("mappings");
        if (root == null) return;

        foreach (var typeName in ResourceTypes.Keys)
        {
            var typeMapping = root.ElementIgnoreCase(typeName);
            if (typeMapping == null) continue;

            foreach (var mapping in typeMapping.Elements("mapping"))
            {
                var key = mapping.Attribute("key");
                var name = mapping.Attribute("name");
                if (key == null || name == null) continue;

                var resourceKey = new ResourceKey();
                var keyParts = key.Value.Split(":");

                if (keyParts.Length != 3) continue;

                resourceKey.Type = Convert.ToUInt32(keyParts[0], 16);
                resourceKey.Group = Convert.ToUInt32(keyParts[1], 16);
                resourceKey.Instance = Convert.ToUInt64(keyParts[2], 16);

                FileTranslator.AddTranslation(resourceKey, name.Value + "." + typeName.ToLower());
            }
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

    public override void RegisterAssetExporters(ExporterService exporter)
    {
        exporter.RegisterExporter(new DdsType(), ".png", TextureExporter.Create(TextureLoader.ImageType.Png));
        exporter.RegisterExporter(new DdsType(), ".dds", TextureExporter.Create(TextureLoader.ImageType.Dds));
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
        foreach (var enumerateFile in Directory.EnumerateFiles(SaveLocation, "*.sav", SearchOption.AllDirectories))
        {
            var saveData = new MySimsCozyBundleSaveGameLoader().Load(new CozyBundleSaveFileSystem(enumerateFile));
            
            if (saveData != null)
                saves.Saves.Add(saveData);
        }
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