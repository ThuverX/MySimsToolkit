using System;
using System.IO;
using System.Threading.Tasks;
using Godot;
using MySimsToolkit.Scripts.AssetLoaders;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetExporters;

public static class TextureExporter
{
    public static Func<IFileSystem.FileId, Task<Stream>> Create(TextureLoader.ImageType imageType)
    {
        return (fileId) => JobService.Instance.Run(async () =>
        {
            var texture = await AssetService.Instance.Load<Texture2D>(fileId).AsTask();

            var bytes = imageType switch
            {
                TextureLoader.ImageType.Png => texture.GetImage().SavePngToBuffer(),
                TextureLoader.ImageType.Dds => texture.GetImage().SaveDdsToBuffer(),
                TextureLoader.ImageType.Jpeg => texture.GetImage().SaveJpgToBuffer(),
                _ => throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null)
            };

            return (Stream) new MemoryStream(bytes);
        });
    }
}