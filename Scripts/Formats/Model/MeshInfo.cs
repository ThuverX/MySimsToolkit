using System.Collections.Generic;
using MySimsToolkit.Scripts.Formats.FileSystem;

namespace MySimsToolkit.Scripts.Formats.Model;

// Mesh as represented by Godot
public record MeshInfo
{
    public List<VertexInfo> Vertices { get; } = [];
    public List<TriangleInfo> Triangles { get; } = [];
    public IFileSystem.FileId Material { get; set; }
}