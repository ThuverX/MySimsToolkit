using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class TplType : FileType
{
    public override string Name => "TPL";
    public override string[] Extensions => [".tpl", ".0x92aa4d6a"];
    public override string Icon => Neat.Icons.FileImage;
    public override string Description => "A TPL texture file.";
}