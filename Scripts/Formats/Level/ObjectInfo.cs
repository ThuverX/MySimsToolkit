using Godot;
using MySimsToolkit.Scripts.Formats.FileSystem;

namespace MySimsToolkit.Scripts.Formats.Level;

public record ObjectInfo
{
    public IFileSystem.FileId FileId { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }
}