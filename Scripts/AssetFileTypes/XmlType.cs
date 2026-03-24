using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class XmlType : FileType
{
    public override string Name => "XML";
    public override string[] Extensions => [".xml"];
    public override string Icon => Neat.Icons.FileCode;
    public override string Description => "A generic XML file.";
}