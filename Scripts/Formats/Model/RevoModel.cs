using System.Collections.Generic;
using System.Diagnostics;
using Godot;
using MySimsToolkit.Scripts.Extensions;

namespace MySimsToolkit.Scripts.Formats.Model;

public record RevoModel : IModel, IBinaryReadable<RevoModel>
{
    public List<MeshInfo> Meshes => [];

    public record Drawable
    {
        public Vector3 BoundsMin;
        public Vector3 BoundsMax;
    }
    
    public List<Drawable> Drawables = [];

    public static RevoModel Read(EndiannessAwareBinaryReader binaryReader)
    {
        var model = new RevoModel();
        
        var rmdl = binaryReader.ReadString(4);
    
        Trace.Assert(rmdl == "RMDL", $"Expected magic to be \"RMDL\" but got \"{rmdl}\"");

        binaryReader.ReadUInt32(); // flags
        
        binaryReader.ReadUInt32();
        binaryReader.ReadUInt32();
        binaryReader.ReadUInt32();
        binaryReader.ReadUInt32();
        binaryReader.ReadUInt32();
        binaryReader.ReadUInt32();
        
        var numDrawables = binaryReader.ReadUInt32();
        var drawablesOffset = binaryReader.ReadUInt32();

        using (binaryReader.Jump(drawablesOffset))
        {
            
        }
        
        return model;
    }
}