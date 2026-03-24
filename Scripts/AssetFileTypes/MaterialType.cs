using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class MaterialType : FileType
{
    public override string Name => "Material";
    public override string[] Extensions => [".material", ".0x01d0e75d"];
    public override string Icon => Neat.Icons.FileDigit;
    public override string Description => "A single material with material parameters.";
}