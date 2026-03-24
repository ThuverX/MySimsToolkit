using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class MaterialSetType : FileType
{
    public override string Name => "MaterialSet";
    public override string[] Extensions => [];
    public override string Icon => Neat.Icons.FileDigit;
    public override string Description => "A collection of materials.";
}