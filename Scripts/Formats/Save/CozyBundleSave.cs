using System.Collections.Generic;
using System.IO;
using MySimsToolkit.Scripts.Extensions;

namespace MySimsToolkit.Scripts.Formats.Save;

public class CozyBundleSave
{
    public Dictionary<string, FileEntry> Entries { get; set; } = [];
    
    public class FileEntry
    {

        public string Path { get; set; }
        public long Size { get; set; }
        public long UncompressedSize { get; set; }
        public long Offset { get; set; }
        
        public static FileEntry Read(EndiannessAwareBinaryReader binaryReader)
        {
            var entry = new FileEntry();
            
            var nameLength = binaryReader.ReadInt32();
            entry.Path = binaryReader.ReadString(nameLength);
            
            entry.Size = binaryReader.ReadInt64();
            entry.UncompressedSize = binaryReader.ReadInt64();

            entry.Offset = binaryReader.BaseStream.Position;
            binaryReader.BaseStream.Seek(entry.Size, SeekOrigin.Current);

            binaryReader.ReadByte();

            return entry;
        }
    }
    
    public static CozyBundleSave Read(EndiannessAwareBinaryReader binaryReader)
    {
        var save = new CozyBundleSave();
        
        var fileCount = binaryReader.ReadInt32();

        for (var i = 0; i < fileCount; i++)
        {
            var entry = FileEntry.Read(binaryReader);
            save.Entries.Add(entry.Path, entry);
        }

        return save;
    }
}