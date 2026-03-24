using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class DdsType : FileType
{
    public override string Name => "DDS";
    public override string[] Extensions => [".dds", ".0x00b2d882"];
    public override string Icon => Neat.Icons.FileImage;
    public override string Description => "A DDS texture file.";
}