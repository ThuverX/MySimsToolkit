using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class ExecutableType : FileType
{
    public override string Name => "Executable";
    public override string[] Extensions => [".exe"];
    public override string Icon => Neat.Icons.FileCog;
    public override string Description => "An executable.";
}