using System.Collections.Generic;

namespace MySimsToolkit.Scripts.Formats.DBPF;

public class DbpfIndexV3002 : IDbpfIndex
{
    public record IndexType
    {
        public required uint Type;
        public required uint Group;
        public required uint Count;
    }
    
    public Dictionary<ResourceKey, DbpfIndex> Entries { get; } = [];
    public List<IndexType> IndexTypes = [];
    public static DbpfIndexV3002 Read(EndiannessAwareBinaryReader binaryReader)
    {
        var indexHeader = new DbpfIndexV3002();

        var numTypes = binaryReader.ReadUInt64();
        for (ulong i = 0; i < numTypes; i++)
        {
            indexHeader.IndexTypes.Add(new IndexType
            {
                Type = binaryReader.ReadUInt32(),
                Group =  binaryReader.ReadUInt32(),
                Count = binaryReader.ReadUInt32()
            });

            binaryReader.ReadUInt32();
        }
        
        foreach (var indexType in indexHeader.IndexTypes)
        {
            for (var i = 0; i < indexType.Count; i++)
            {
                var isCompressed = indexType.Group != 0;
                var instance = binaryReader.ReadUInt64();
                var offset = binaryReader.ReadUInt32();
                var size = binaryReader.ReadUInt32();
                if (isCompressed) { 
                    binaryReader.ReadUInt32();
                }

                var key = new ResourceKey
                {
                    Instance = instance,
                    Group = indexType.Group,
                    Type = indexType.Type,
                };
                
                indexHeader.Entries.TryAdd(key, new DbpfIndex
                {
                    Key = key,
                    Offset = offset,
                    Size = size,
                    IsCompressed = isCompressed,
                });
            }
        }
        
        return indexHeader;
    }
}