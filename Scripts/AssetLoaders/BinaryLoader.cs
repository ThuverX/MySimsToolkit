using System;
using System.Threading.Tasks;
using MySimsToolkit.Scripts.Formats;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetLoaders;

public static class BinaryLoader
{
    public static Func<IFileSystem.FileId, Task<TContract>> Create<TContract, TConcrete>()
        where TConcrete : TContract, IBinaryReadable<TConcrete>
    {
        return (fileId) =>
            JobService.Instance.Run(() =>
            {
                using var stream = FileSystemService.Instance.OpenRead(fileId);
                using var reader = new EndiannessAwareBinaryReader(stream);
                return (TContract)TConcrete.Read(reader);
            });
    }
}