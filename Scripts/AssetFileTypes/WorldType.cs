using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class WorldType : FileType
{
    public override string Name => "World";
    public override string[] Extensions => [".world"];
    public override string Icon => Neat.Icons.FileAxis_3d;
    public override string Description => "A game world with a level and objects.";
}