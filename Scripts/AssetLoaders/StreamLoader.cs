using System;
using System.Threading.Tasks;
using MySimsToolkit.Scripts.Formats;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetLoaders;

public static class StreamLoader
{
    public static Func<IFileSystem.FileId, Task<TContract>> Create<TContract, TConcrete>()
        where TConcrete : TContract, IStreamReadable<TConcrete>
    {
        return (fileId) =>
            JobService.Instance.Run(() =>
            {
                using var stream = FileSystemService.Instance.OpenRead(fileId);
                return (TContract)TConcrete.Read(stream);
            });
    }
}