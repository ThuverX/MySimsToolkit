using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.Saves;

public interface ISaveGameLoader
{
    SaveService.SaveFileData Load(IFileSystem vfs);
}