using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class ShaderType : FileType
{
    public override string Name => "Shader";
    public override string[] Extensions => [".fx", ".dxil"];
    public override string Icon => Neat.Icons.FileDigit;
    public override string Description => "A shader file.";
}