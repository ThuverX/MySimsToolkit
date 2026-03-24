using System.Collections.Generic;
using MySimsToolkit.Scripts.Formats;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Formats.FileSystem.FileId;
using MySimsToolkit.Scripts.Nodes;

namespace MySimsToolkit.Scripts.Services;

public class FileTranslatorService
{
    public static FileTranslatorService Instance => RuntimeRoot.Instance.Runtime.Translator;
    public Dictionary<ResourceKey, string> ResourceNames = [];
    
    public void AddTranslation(ResourceKey resourceKey, string name)
    {
        ResourceNames[resourceKey] = name;
    }
    
    public string GetNameForFileId(IFileSystem.FileId fileId)
    {
        if (fileId is IHasResourceKey resourceKeyId &&
            ResourceNames.TryGetValue(resourceKeyId.Key, out var resourceName))
            return resourceName.ToLower();
        return fileId.ToString();
    }
}