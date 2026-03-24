using System.Collections.Generic;
using System.IO;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Formats.FileSystem.FileId;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.UI.Views.Browser;

public class FileHierarchy
{
    public class BrowserFolder
    {
        public string Name { get; init; }
        public Dictionary<string, BrowserFolder> Folders { get; } = [];
        public List<BrowserFile> Files { get; } = [];
    }

    public class BrowserFile
    {
        public string? Group;
        public IFileSystem.FileId Id { get; init; }
        public IFileSystem Source { get; init; }
    }
    
    public static BrowserFolder Build()
    {
        var root = new BrowserFolder { Name = "/" };

        foreach (var mount in FileSystemService.Instance.Mounts)
        {
            if (mount is not IHasRootPath rootMount) continue;
            
            var relativePath = Path.GetRelativePath(FileSystemService.Instance.RootPath, rootMount.RootPath);
            var folder = EnsureFolder(root, relativePath);

            foreach (var fileId in mount.Enumerate())
            {
                string group = null;
                if (fileId is IHasResourceKey resourceKey && resourceKey.Key.Group != 0)
                {
                    group = "0x" + resourceKey.Key.Group.ToString("X8");
                }
                
                folder.Files.Add(new BrowserFile
                {
                    Id = fileId,
                    Group = group,
                    Source = mount
                });
            }
        }

        return root;
    }
    
    private static BrowserFolder GetOrCreate(BrowserFolder parent, string path)
    {
        if (parent.Folders.TryGetValue(path, out var folder))
        {
            return folder;
        }
        folder = new BrowserFolder { Name = path };
        parent.Folders.TryAdd(path, folder);
        
        return folder;
    }

    private static BrowserFolder EnsureFolder(BrowserFolder root, string path)
    {
        if (path == ".") return root;
        
        var parts = path.Split(Path.DirectorySeparatorChar);
        if (parts.Length <= 1)
        {
            return GetOrCreate(root, path);
        }

        var bottomLevelFolder = root;
        
        foreach (var part in parts)
        {
            var newFolder = GetOrCreate(bottomLevelFolder, part);
            
            bottomLevelFolder = newFolder;
        }
        
        return bottomLevelFolder;
    }
}