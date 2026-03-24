using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class FontType : FileType
{
    public override string Name => "Font";
    public override string[] Extensions => [".ttf", ".otf", ".ttc"];
    public override string Icon => Neat.Icons.FileType;
    public override string Description => "A font file.";
}