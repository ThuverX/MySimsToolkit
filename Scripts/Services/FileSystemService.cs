using System;
using System.Collections.Generic;
using MySimsToolkit.Scripts.AssetFileTypes;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Nodes;

namespace MySimsToolkit.Scripts.Services;

public class FileSystemService(string rootPath) : VirtualFileSystem
{
    public static FileSystemService Instance => RuntimeRoot.Instance.Runtime.FileSystem;


    public readonly List<FileType> FileTypes = [];

    private readonly Dictionary<string, FileType> _fileTypeAccelerationStructure = [];

    public FileType GetFileTypeByExtension(string extension)
    {
        if (_fileTypeAccelerationStructure.Count <= 0)
        {
            foreach (var fileType in FileTypes)
            {
                foreach (var fileTypeExtension in fileType.Extensions)
                {
                    _fileTypeAccelerationStructure.TryAdd(fileTypeExtension.ToLower(), fileType);
                }
            }
        }

        return _fileTypeAccelerationStructure.GetValueOrDefault(extension.ToLower(), new GenericType());
    }

    public string RootPath { get; private set; } = rootPath;
}