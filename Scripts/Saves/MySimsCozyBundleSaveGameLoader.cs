using System.IO;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Formats.FileSystem.FileId;
using MySimsToolkit.Scripts.Formats.Save;
using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.Saves;

public class MySimsCozyBundleSaveGameLoader : ISaveGameLoader
{
    public SaveService.SaveFileData Load(IFileSystem vfs)
    {
        if (vfs is not IHasRootPath rootPathVfs) return null;
        
        var save = SaveHeaderXml.Read(vfs.OpenRead(new PathFileId("SaveHeader.xml")));

        var filename = Path.GetFileName(rootPathVfs.RootPath)!.Split(".")[0];
        var slot = filename switch
        {
            "SaveData1" => 1,
            "SaveData2" => 2,
            "SaveData3" => 3,
            _ => -1
        };

        return new SaveService.SaveFileData(slot, save.TownName, save.SimName, vfs, save);
    }
}