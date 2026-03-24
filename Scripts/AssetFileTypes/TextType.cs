using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class TextType : FileType
{
    public override string Name => "Text";
    public override string[] Extensions => [".txt"];
    public override string Icon => Neat.Icons.FileText;
    public override string Description => "A text file.";
}