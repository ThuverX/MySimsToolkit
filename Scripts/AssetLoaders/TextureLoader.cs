using System;
using System.IO;
using System.Threading.Tasks;
using Godot;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Formats.Nintendo;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetLoaders;

public static class TextureLoader
{
    public enum ImageType
    {
        Dds,
        Png,
        Tga,
        Tpl,
        Jpeg,
        Bmp
    }
    
    public static Func<IFileSystem.FileId, Task<Texture2D>> Create(ImageType imageType)
    {
        return (fileId) => JobService.Instance.Run(async () =>
        {
            await using var stream = FileSystemService.Instance.OpenRead(fileId);

            var buffer = new byte[stream.Length];
            await stream.ReadExactlyAsync(buffer, 0, buffer.Length);
            
            await JobService.Instance.SwitchToMainThread();

            var image = new Image();

            switch (imageType)
            {
                case ImageType.Dds:
                    image.LoadDdsFromBuffer(buffer);
                    break;
                case ImageType.Png:
                    image.LoadPngFromBuffer(buffer);
                    break;
                case ImageType.Tga:
                    image.LoadTgaFromBuffer(buffer);
                    break;
                case ImageType.Jpeg:
                    image.LoadJpgFromBuffer(buffer);
                    break;
                case ImageType.Bmp:
                    image.LoadBmpFromBuffer(buffer);
                    break;
                case ImageType.Tpl:
                {
                    using var bufferStream = new MemoryStream(buffer);
                    using var reader = new EndiannessAwareBinaryReader(bufferStream);
                    var file = TplFile.Read(reader);

                    if (file.TplImages.Count > 0)
                    {
                        var img = file.TplImages[0];
                        image = Image.CreateFromData(img.Width, img.Height, false, Image.Format.Rgba8, img.Data);
                    }
                    
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(imageType), imageType, null);
            }
            
            image.SavePng("E:/export/" + fileId + ".png");

            return (Texture2D)ImageTexture.CreateFromImage(image);
        });
    }
}