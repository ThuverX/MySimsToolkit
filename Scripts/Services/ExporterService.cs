using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MySimsToolkit.Scripts.AssetFileTypes;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Nodes;

namespace MySimsToolkit.Scripts.Services;

public class ExporterService
{
    public static ExporterService Instance => RuntimeRoot.Instance.Runtime.Exporter;

    private readonly Dictionary<FileType, Dictionary<string, Func<IFileSystem.FileId, Task<Stream>>>> _exporters = new();

    public void RegisterExporter(FileType type, string extension, Func<IFileSystem.FileId, Task<Stream>> loader)
    {
        if (!_exporters.TryGetValue(type, out var value))
        {
            value = new Dictionary<string, Func<IFileSystem.FileId, Task<Stream>>>();
            _exporters.Add(type, value);
        }

        value[extension] = async id => await loader(id);
    }

    public Func<IFileSystem.FileId, Task<Stream>> GetExporter(FileType type, string extension)
    {
        return !_exporters.TryGetValue(type, out var types) ? null : types.GetValueOrDefault(extension, null);
    }

    public async void Export(FileType type, IFileSystem.FileId fileId, string path)
    {
        var exporter = GetExporter(type, Path.GetExtension(path));

        if (exporter == null)
        {
            return;
        }

        await using var stream = await exporter(fileId);
        await using var fileStream = File.Create(path);
        await stream.CopyToAsync(fileStream);
    }

    public string[] GetOptions(FileType type)
    {
        return !_exporters.TryGetValue(type, out var types) ? [] : types.Keys.ToArray();
    }
}