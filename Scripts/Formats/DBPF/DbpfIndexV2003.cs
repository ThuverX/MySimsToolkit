using System;
using System.Collections.Generic;

namespace MySimsToolkit.Scripts.Formats.DBPF;

public class DbpfIndexV2003 : IDbpfIndex
{
    [Flags]
    public enum IndexFieldFlags
    {
        ResourceType = 1 << 0,
        ResourceGroup = 1 << 1,
        InstanceHi = 1 << 2,
        InstanceLo = 1 << 3,
        Offset = 1 << 4,
        FileSize = 1 << 5,
        MemSize = 1 << 6,
        Flags = 1 << 7
    }
    
    public Dictionary<ResourceKey, DbpfIndex> Entries { get; } = [];

    public IndexFieldFlags FieldFlags;

     public uint? EntryFileType;
     public uint? EntryGroup;
     public uint? EntryInstanceHi;
     public uint? EntryInstanceLo;
    
    public static DbpfIndexV2003 Read(EndiannessAwareBinaryReader binaryReader, uint entryCount)
    {
        var indexHeader = new DbpfIndexV2003
        {
            FieldFlags = (IndexFieldFlags)binaryReader.ReadUInt32()
        };

        if (indexHeader.FieldFlags.HasFlag(IndexFieldFlags.ResourceType))
            indexHeader.EntryFileType = binaryReader.ReadUInt32();

        if (indexHeader.FieldFlags.HasFlag(IndexFieldFlags.ResourceGroup))
            indexHeader.EntryGroup = binaryReader.ReadUInt32();

        if (indexHeader.FieldFlags.HasFlag(IndexFieldFlags.InstanceHi))
            indexHeader.EntryInstanceHi = binaryReader.ReadUInt32();

        if (indexHeader.FieldFlags.HasFlag(IndexFieldFlags.InstanceLo))
            indexHeader.EntryInstanceLo = binaryReader.ReadUInt32();

        for (var i = 0; i < entryCount; i++)
        {
            var key = new ResourceKey
            {
                Type = indexHeader.EntryFileType ?? binaryReader.ReadUInt32(),
                Group = indexHeader.EntryGroup ?? binaryReader.ReadUInt32(),
                Instance = ((ulong)(indexHeader.EntryInstanceHi ?? binaryReader.ReadUInt32()) << 32) |
                           (indexHeader.EntryInstanceLo ?? binaryReader.ReadUInt32())
            };
            var offset = binaryReader.ReadUInt32();
            var fileSize = binaryReader.ReadUInt32() & 0x7FFFFFFF;
            var memSize = binaryReader.ReadUInt32();
            var flags = binaryReader.ReadUInt16();
            
            var isCompressed = flags == 0xffff;
            binaryReader.ReadUInt16();
            
            indexHeader.Entries.TryAdd(key, new DbpfIndex
            {
                Key = key,
                Offset = offset,
                Size = memSize,
                IsCompressed = isCompressed,
            });
        }
        
        return indexHeader;
    }
}