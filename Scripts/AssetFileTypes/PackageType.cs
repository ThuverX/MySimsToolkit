using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class PackageType : FileType
{
    public override string Name => "Package";
    public override string[] Extensions => [".package"];
    public override string Icon => Neat.Icons.FileArchive;
    public override string Description => "A packed game database.";
}