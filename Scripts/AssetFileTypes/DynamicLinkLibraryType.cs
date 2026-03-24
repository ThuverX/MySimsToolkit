using MySimsToolkit.Scripts.Services;

namespace MySimsToolkit.Scripts.AssetFileTypes;

public class DynamicLinkLibraryType : FileType
{
    public override string Name => "DLL";
    public override string[] Extensions => [".dll"];
    public override string Icon => Neat.Icons.FileCog;
    public override string Description => "A Dynamic Link Library.";
}