namespace MySimsToolkit.Scripts.Formats.DBPF;

public record DbpfIndex
{
    public required ResourceKey Key;
    public required uint Offset;
    public required uint Size;
    public required bool IsCompressed;
}