using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Godot;
using MySimsToolkit.Scripts.Extensions;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Formats.FileSystem.FileId;

namespace MySimsToolkit.Scripts.Formats.Level;

public record LevelBin : ILevel, IBinaryReadable<LevelBin>
{

    public List<IFileSystem.FileId> Parts { get; } = [];
    public List<ObjectInfo> Objects { get; } = [];
    
    public float StartPosX { get; set; }
    public float StartPosY { get; set; }
    
    public uint CellSizeX { get; set; }
    public uint CellSizeY { get; set; }
    
    public uint NumCellsX { get; set; }
    public uint NumCellsY { get; set; }
    
    public Vector3 BoundsMin;
    public Vector3 BoundsMax;

    public static LevelBin Read(EndiannessAwareBinaryReader binaryReader)
    {
        var llmf = binaryReader.ReadString(4);

        Trace.Assert(llmf == "LLMF", $"Expected LLMF but got \"{llmf}\"");

        binaryReader.ReadUInt32();
        
        var level = new LevelBin()
        {
            StartPosX = binaryReader.ReadSingle(),
            StartPosY = binaryReader.ReadSingle(),
            
            CellSizeX = binaryReader.ReadUInt32(),
            CellSizeY = binaryReader.ReadUInt32(),
            
            NumCellsX = binaryReader.ReadUInt32(),
            NumCellsY = binaryReader.ReadUInt32(),
        };

        binaryReader.ReadUInt32();
        binaryReader.ReadUInt32();
        
        var numModels = binaryReader.ReadUInt32();
        var numObjects = binaryReader.ReadUInt32();

        level.BoundsMin = binaryReader.ReadVector3();
        level.BoundsMax = binaryReader.ReadVector3();

        for (var i = 0; i < numModels; i++)
        {
            var instance = binaryReader.ReadUInt64();
            var key = new ResourceKey
            {
                Instance = instance,
                Type = 0xb359c791,
                Group = 0
            };
            
            level.Parts.Add(new ResourceKeyFileId(key));
        }

        for (var i = 0; i < numObjects; i++)
        {
            level.Objects.Add(ReadObject(binaryReader));
        }
        
        return level;
    }
    
    public static ObjectInfo ReadObject(EndiannessAwareBinaryReader binaryReader)
    {
        var obj = new ObjectInfo();
            
        var instance = binaryReader.ReadUInt64();
        var key = new ResourceKey
        {
            Instance = instance,
            Type = 0xb359c791,
            Group = 0
        };
        obj.FileId = new ResourceKeyFileId(key);

        obj.Position = binaryReader.ReadVector3();
        obj.Rotation = binaryReader.ReadVector3();
        obj.Scale = binaryReader.ReadVector3();
        
        binaryReader.ReadUInt32();
            
        return obj;
    }
}