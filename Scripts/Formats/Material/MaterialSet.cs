using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MySimsToolkit.Scripts.Extensions;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Formats.FileSystem.FileId;

namespace MySimsToolkit.Scripts.Formats.Material;

public record MaterialSet : IMaterialSet
{
    public List<IFileSystem.FileId> MaterialReferences { get; set; } = [];
    
    public static MaterialSet Read(EndiannessAwareBinaryReader binaryReader, MaterialVersion materialVersion)
    {
        var materialSet = new MaterialSet();

        if (materialVersion == MaterialVersion.MySims)
        {
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();

            var numMaterials = binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();

            ResourceKey.Read(binaryReader);

            for (var i = 0; i < numMaterials; i++)
            {
                materialSet.MaterialReferences.Add(new ResourceKeyFileId(ResourceKey.Read(binaryReader)));
            }

            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();

            var mtst = binaryReader.ReadString(4);

            Trace.Assert(mtst == "MTST", $"Expected mtst but got \"{mtst}\"");

            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();

            for (var i = 0; i < numMaterials; i++)
            {
                binaryReader.ReadUInt32();
            }
        }
        else
        {
            var mtst = binaryReader.ReadString(4);

            Trace.Assert(mtst == "MTST", $"Expected mtst but got \"{mtst}\"");
            
            binaryReader.ReadUInt32();
            binaryReader.ReadUInt32();
            
            var numMaterials = binaryReader.ReadUInt32();
            
            for (var i = 0; i < numMaterials; i++)
            {
                var instance = binaryReader.ReadUInt64();
                var key = new ResourceKey
                {
                    Group = 0,
                    Instance = instance,
                    Type = materialVersion == MaterialVersion.Kingdom ? 0x01d0e75d : 0xe6640542
                };
                
                materialSet.MaterialReferences.Add(new ResourceKeyFileId(key));
            }
        }

        return materialSet;
    }

    public List<IFileSystem.FileId> Materials => MaterialReferences;
}