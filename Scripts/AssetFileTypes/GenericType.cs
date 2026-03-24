using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class GenericType : FileType
{
    public override string Name => "Generic";
    public override string[] Extensions => [];
    public override string Icon => Neat.Icons.File;
}