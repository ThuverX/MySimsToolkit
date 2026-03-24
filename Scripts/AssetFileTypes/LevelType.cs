using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class LevelType : FileType
{
    public override string Name => "Level";

    public override string[] Extensions =>
        [".levelxml", ".levelbin", ".0x585ee310", ".0x58969018"];

    public override string Icon => Neat.Icons.FileAxis_3d;
    public override string Description => "A Level file.";
}