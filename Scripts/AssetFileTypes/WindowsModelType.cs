using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class WindowsModelType : FileType
{
    public override string Name => "Windows model";
    public override string[] Extensions => [".wmdl", ".0xb359c791"];
    public override string Icon => Neat.Icons.FileBox;
    public override string Description => "A 3D model.";
}