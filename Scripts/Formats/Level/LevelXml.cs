using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using MySimsToolkit.Scripts.Formats.FileSystem;
using MySimsToolkit.Scripts.Formats.FileSystem.FileId;

namespace MySimsToolkit.Scripts.Formats.Level;

public record LevelXml : ILevel, IStreamReadable<LevelXml>
{
    public List<IFileSystem.FileId> Parts { get; } = [];
    public List<ObjectInfo> Objects { get; } = [];

    public static LevelXml Read(Stream stream)
    {
        var level = new LevelXml();

        var doc = XDocument.Load(stream);
        
        var levelNode = doc.Element("Level");
        
        var gridCellsNode = levelNode!.Element("GridCells");

        var modelNodes = gridCellsNode!.Elements("Model");
        
        foreach (var modelNode in modelNodes)
        {
            var modelId = Convert.ToUInt32(modelNode.Value);
            var key = new ResourceKey
            {
                Group = modelId,
                Instance = 1,
                Type = 0xb359c791
            };
            
            level.Parts.Add(new ResourceKeyFileId(key));
        }
        
        return level;
    }
}