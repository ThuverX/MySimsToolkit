using Godot;

namespace MySimsToolkit.Scripts.Formats.Model;

// Vertex as represented by Godot
public record VertexInfo
{
    public Vector3 Position { get; set; }
    public Vector3 Normal { get; set; }
    public Vector2 TexCoord1 { get; set; }
    public Vector2 TexCoord2 { get; set; }
}