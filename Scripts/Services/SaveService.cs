using System.Collections.Generic;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Nodes;

namespace MySimsToolkit.Scripts.Services;

public class SaveService
{
    public static SaveService Instance => RuntimeRoot.Instance.Runtime.Saves;

    public record SaveFileData(int Index, string Name, string PlayerName, IFileSystem Vfs, object Data)
    {
        public T GetData<T>() where T : new()
        {
            return (T) Data;
        }
    };
    
    public VirtualFileSystem Vfs = new();
    public List<SaveFileData> Saves = [];
}