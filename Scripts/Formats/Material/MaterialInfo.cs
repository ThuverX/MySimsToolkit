using Godot;
using MySimsToolkit.Scripts.Formats.FileSystem;

namespace MySimsToolkit.Scripts.Formats.Material;

public record MaterialInfo
{
    public IFileSystem.FileId MainTexture { get; set; }
    public Color TintColor { get; set; }
}